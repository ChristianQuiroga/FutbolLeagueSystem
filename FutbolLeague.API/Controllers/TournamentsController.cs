using FutbolLeague.Application.DTOs;
using FutbolLeague.Domain;
using FutbolLeague.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//Ctrol + M, O para colapsar todo el código y tener una vista general del controlador
namespace FutbolLeague.API.Controllers
{
    /// Controller for managing tournaments in the football league application
    [ApiController]
    [Route("api/[controller]")]
    public class TournamentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TournamentsController(AppDbContext context)
        {
            _context = context;
        }


        // GET: api/tournaments
        // Devuelve la lista de torneos disponibles con su formato de fixture
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tournaments = await _context.Tournaments
                .Select(t => new TournamentDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    FixtureFormat = (int)t.FixtureFormat,
                    FixtureFormatName = t.FixtureFormat.ToString()
                })
                .ToListAsync();

            return Ok(tournaments);
        }


        // POST: api/tournaments
        // Crea un nuevo torneo con el nombre y formato de fixture especificados
        [HttpPost]
        public async Task<IActionResult> Create(CreateTournamentDto dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
                return BadRequest("El nombre es obligatorio");

            if (!Enum.IsDefined(typeof(FixtureFormat), dto.FixtureFormat))
                return BadRequest("Formato de fixture inválido");

            // Verificar si ya existe un torneo con el mismo nombre (ignorando mayúsculas)
            var exists = await _context.Tournaments
                .AnyAsync(t => t.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                return BadRequest("Ya existe un torneo con ese nombre");
            //

            var tournament = new Tournament
            {
                Name = dto.Name,
                FixtureFormat = (FixtureFormat)dto.FixtureFormat
            };

            _context.Tournaments.Add(tournament);
            await _context.SaveChangesAsync();

            return Ok(new TournamentDto
            {
                Id = tournament.Id,
                Name = tournament.Name,
                FixtureFormat = (int)tournament.FixtureFormat,
                FixtureFormatName = tournament.FixtureFormat.ToString()
            });
        }



        // GET: api/tournaments/{tournamentId}/summary/{categoryId}
        // Devuelve un resumen del torneo para una categoría específica, incluyendo próximos partidos, últimos resultados, standings y estadísticas
        [HttpGet("{tournamentId}/summary/{categoryId}")]
        public async Task<IActionResult> GetSummary(int tournamentId, int categoryId)
        {
            // Equipos
            var teams = await _context.Teams
                .Where(t => t.TournamentId == tournamentId && t.CategoryId == categoryId)
                .ToListAsync();

            if (!teams.Any())
                return NotFound("No hay equipos para ese torneo y categoría");

            var teamIds = teams.Select(t => t.Id).ToList();

            // Partidos
            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.Field)
                .Where(m => m.TournamentId == tournamentId &&
                            teamIds.Contains(m.HomeTeamId) &&
                            teamIds.Contains(m.AwayTeamId))
                .ToListAsync();

            if (!matches.Any())
                return NotFound("No hay partidos para ese torneo y categoría");

            // Próximos partidos (Pending)
            var nextMatches = matches
                .Where(m => m.Status == MatchStatus.Pending)
                .OrderBy(m => m.MatchDate)
                .Take(5)
                .Select(m => new MatchDto
                {
                    Id = m.Id,
                    Round = m.Round,
                    HomeTeam = m.HomeTeam.Name,
                    AwayTeam = m.AwayTeam.Name,
                    MatchDate = m.MatchDate,
                    Status = m.Status.ToString(),
                    Field = m.Field != null ? m.Field.Name : null
                })
                .ToList();

            // Últimos resultados (Played)
            var lastResults = matches
                .Where(m => m.Status == MatchStatus.Played)
                .OrderByDescending(m => m.MatchDate)
                .Take(5)
                .Select(m => new MatchDto
                {
                    Id = m.Id,
                    Round = m.Round,
                    HomeTeam = m.HomeTeam.Name,
                    AwayTeam = m.AwayTeam.Name,
                    HomeScore = m.HomeScore,
                    AwayScore = m.AwayScore,
                    MatchDate = m.MatchDate,
                    Status = m.Status.ToString(),
                    Field = m.Field != null ? m.Field.Name : null
                })
                .ToList();

            // STANDINGS
            var standings = teams
                .Select(team =>
                {
                    var playedMatches = matches
                        .Where(m => m.Status == MatchStatus.Played &&
                                   (m.HomeTeamId == team.Id || m.AwayTeamId == team.Id))
                        .ToList();

                    int points = 0;
                    int goalsFor = 0;
                    int goalsAgainst = 0;

                    foreach (var match in playedMatches)
                    {
                        bool isHome = match.HomeTeamId == team.Id;

                        int scored = isHome ? match.HomeScore ?? 0 : match.AwayScore ?? 0;
                        int conceded = isHome ? match.AwayScore ?? 0 : match.HomeScore ?? 0;

                        goalsFor += scored;
                        goalsAgainst += conceded;

                        if (scored > conceded) points += 3;
                        else if (scored == conceded) points += 1;
                    }

                    return new
                    {
                        Team = team.Name,
                        Points = points,
                        GoalDifference = goalsFor - goalsAgainst,
                        GoalsFor = goalsFor
                    };
                })
                .OrderByDescending(s => s.Points)
                .ThenByDescending(s => s.GoalDifference)
                .ThenByDescending(s => s.GoalsFor)
                .ToList();

            // STATS
            var totalMatches = matches.Count;
            var playedMatchesCount = matches.Count(m => m.Status == MatchStatus.Played);
            var pendingMatchesCount = matches.Count(m => m.Status == MatchStatus.Pending);

            return Ok(new
            {
                TournamentId = tournamentId,
                CategoryId = categoryId,
                NextMatches = nextMatches,
                LastResults = lastResults,
                Standings = standings,
                Stats = new
                {
                    TotalMatches = totalMatches,
                    PlayedMatches = playedMatchesCount,
                    PendingMatches = pendingMatchesCount
                }
            });
        }
    }
}
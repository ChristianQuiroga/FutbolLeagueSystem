using FutbolLeague.Application.DTOs;
using FutbolLeague.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FutbolLeague.API.Controllers
{
    // Controlador para manejar las posiciones de los equipos en el torneo
    [ApiController]
    [Route("api/[controller]")]
    public class StandingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StandingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Standings/tournament/5
        // Obtener la tabla de posiciones para un torneo específico
        [HttpGet("tournament/{tournamentId}")]
        public async Task<IActionResult> GetByTournament(int tournamentId)
        {
            // Verificar si el torneo existe
            var teams = await _context.Teams
                .Where(t => t.TournamentId == tournamentId)
                .ToListAsync();

            // Si no hay equipos para el torneo, retornar 404
            if (!teams.Any())
                return NotFound("No hay equipos para ese torneo");

            // Obtener todos los partidos del torneo con resultados registrados
            var matches = await _context.Matches
                .Where(m => m.TournamentId == tournamentId &&
                            m.HomeScore.HasValue &&
                            m.AwayScore.HasValue)
                .ToListAsync();

            // Inicializar las estadísticas para cada equipo
            var standings = teams.Select(team => new StandingDto
            {
                TeamId = team.Id,
                TeamName = team.Name,
                Played = 0,
                Won = 0,
                Drawn = 0,
                Lost = 0,
                GoalsFor = 0,
                GoalsAgainst = 0,
                GoalDifference = 0,
                Points = 0
            }).ToList();

            // Calcular estadísticas para cada equipo basado en los resultados de los partidos
            foreach (var match in matches)
            {
                var homeTeam = standings.First(s => s.TeamId == match.HomeTeamId);
                var awayTeam = standings.First(s => s.TeamId == match.AwayTeamId);

                int homeScore = match.HomeScore!.Value;
                int awayScore = match.AwayScore!.Value;

                homeTeam.Played++;
                awayTeam.Played++;

                homeTeam.GoalsFor += homeScore;
                homeTeam.GoalsAgainst += awayScore;

                awayTeam.GoalsFor += awayScore;
                awayTeam.GoalsAgainst += homeScore;

                if (homeScore > awayScore)
                {
                    homeTeam.Won++;
                    homeTeam.Points += 3;

                    awayTeam.Lost++;
                }
                else if (homeScore < awayScore)
                {
                    awayTeam.Won++;
                    awayTeam.Points += 3;

                    homeTeam.Lost++;
                }
                else
                {
                    homeTeam.Drawn++;
                    awayTeam.Drawn++;

                    homeTeam.Points += 1;
                    awayTeam.Points += 1;
                }
            }

            // Calcular la diferencia de goles para cada equipo
            foreach (var team in standings)
            {
                team.GoalDifference = team.GoalsFor - team.GoalsAgainst;
            }

            // Ordenar por puntos, diferencia de goles, goles a favor y nombre del equipo
            var orderedStandings = standings
                .OrderByDescending(s => s.Points)
                .ThenByDescending(s => s.GoalDifference)
                .ThenByDescending(s => s.GoalsFor)
                .ThenBy(s => s.TeamName)
                .ToList();

            // Asignar posiciones después de ordenar
            for (var i = 0; i < orderedStandings.Count; i++)
            {
                orderedStandings[i].Position = i + 1;
            }

            return Ok(orderedStandings);
        }


        // GET: api/Standings/tournament/5/category/3
        // Obtener la tabla de posiciones para un torneo específico y una categoría específica
        [HttpGet("tournament/{tournamentId}/category/{categoryId}")]
        public async Task<IActionResult> GetByTournamentAndCategory(int tournamentId, int categoryId)
        {
            // Verificar si el torneo y la categoría existen
            var teams = await _context.Teams
                .Where(t => t.TournamentId == tournamentId && t.CategoryId == categoryId)
                .ToListAsync();

            // Si no hay equipos para el torneo y categoría, retornar 404
            if (!teams.Any())
                return NotFound("No hay equipos para ese torneo y categoría");

            var teamIds = teams.Select(t => t.Id).ToList();

            var matches = await _context.Matches
                .Where(m => m.TournamentId == tournamentId &&
                            m.HomeScore.HasValue &&
                            m.AwayScore.HasValue &&
                            teamIds.Contains(m.HomeTeamId) &&
                            teamIds.Contains(m.AwayTeamId))
                .ToListAsync();

            var standings = teams.Select(team => new StandingDto
            {
                Position = 0,
                TeamId = team.Id,
                TeamName = team.Name,
                Played = 0,
                Won = 0,
                Drawn = 0,
                Lost = 0,
                GoalsFor = 0,
                GoalsAgainst = 0,
                GoalDifference = 0,
                Points = 0
            }).ToList();

            foreach (var match in matches)
            {
                var homeTeam = standings.First(s => s.TeamId == match.HomeTeamId);
                var awayTeam = standings.First(s => s.TeamId == match.AwayTeamId);

                int homeScore = match.HomeScore!.Value;
                int awayScore = match.AwayScore!.Value;

                homeTeam.Played++;
                awayTeam.Played++;

                homeTeam.GoalsFor += homeScore;
                homeTeam.GoalsAgainst += awayScore;

                awayTeam.GoalsFor += awayScore;
                awayTeam.GoalsAgainst += homeScore;

                if (homeScore > awayScore)
                {
                    homeTeam.Won++;
                    homeTeam.Points += 3;
                    awayTeam.Lost++;
                }
                else if (homeScore < awayScore)
                {
                    awayTeam.Won++;
                    awayTeam.Points += 3;
                    homeTeam.Lost++;
                }
                else
                {
                    homeTeam.Drawn++;
                    awayTeam.Drawn++;
                    homeTeam.Points += 1;
                    awayTeam.Points += 1;
                }
            }

            foreach (var team in standings)
            {
                team.GoalDifference = team.GoalsFor - team.GoalsAgainst;
            }

            var orderedStandings = standings
                .OrderByDescending(s => s.Points)
                .ThenByDescending(s => s.GoalDifference)
                .ThenByDescending(s => s.GoalsFor)
                .ThenBy(s => s.TeamName)
                .ToList();

            for (int i = 0; i < orderedStandings.Count; i++)
            {
                orderedStandings[i].Position = i + 1;
            }

            return Ok(orderedStandings);
        }
    }

    

}
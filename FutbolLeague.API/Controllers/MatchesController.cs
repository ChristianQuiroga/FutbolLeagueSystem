using FutbolLeague.Application.DTOs;
using FutbolLeague.Domain;
using FutbolLeague.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FutbolLeague.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController : ControllerBase
    {
        // Inyectamos el contexto de la base de datos a través del constructor
        private readonly AppDbContext _context;
        public MatchesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Matches/{id}
        //GET: api/Matches
        //Mejora del método GetAll para incluir el nombre de los equipos y ordenar por ronda
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .OrderBy(m => m.Round)
                .ThenBy(m => m.Id)
                .Select(m => new MatchDto
                {
                    Id = m.Id,
                    Round = m.Round,
                    HomeTeam = m.HomeTeam.Name,
                    AwayTeam = m.AwayTeam.Name,
                    HomeScore = m.HomeScore,
                    AwayScore = m.AwayScore
                })
                .ToListAsync();

            return Ok(matches);
        }


        // Post: api/Matches
        //Mejora del método Create para validar que los equipos no sean iguales y que existan en la base de datos
        [HttpPost]
        public async Task<IActionResult> Create(CreateMatchDto dto)
        {
            if (dto.HomeTeamId == dto.AwayTeamId)
                return BadRequest("Un equipo no puede jugar contra sí mismo");

            var homeExists = await _context.Teams.AnyAsync(t => t.Id == dto.HomeTeamId);
            var awayExists = await _context.Teams.AnyAsync(t => t.Id == dto.AwayTeamId);

            if (!homeExists || !awayExists)
                return BadRequest("Uno de los equipos no existe");

            var match = new Match
            {
                HomeTeamId = dto.HomeTeamId,
                AwayTeamId = dto.AwayTeamId
            };

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            return Ok(match);
        }



        //Get api/Matches/tournament/{tournamentId}/round/{round}
        //Mejora del método GetByRound para incluir el nombre de los equipos y devolver un objeto con la información del torneo, ronda y partidos
        [HttpGet("tournament/{tournamentId}/round/{round}")]
        public async Task<IActionResult> GetByRound(int tournamentId, int round)
        {
            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.TournamentId == tournamentId && m.Round == round)
                .Select(m => new MatchDto
                {
                    Id = m.Id,
                    Round = m.Round,
                    HomeTeam = m.HomeTeam.Name,
                    AwayTeam = m.AwayTeam.Name,
                    HomeScore = m.HomeScore,
                    AwayScore = m.AwayScore
                })
                .ToListAsync();

            if (!matches.Any())
                return NotFound("No hay partidos para esa ronda");

            //return Ok(matches);
            return Ok(new
            {
                TournamentId = tournamentId,
                Round = round,
                Matches = matches
            });
        }

        //Get api/Matches/tournament/{tournamentId}/category/{categoryId}/round/{round}
        //Mejora del método GetByTournamentCategoryAndRound para incluir el nombre de los equipos y devolver un objeto con la información del torneo, categoría, ronda y partidos
        [HttpGet("tournament/{tournamentId}/category/{categoryId}/round/{round}")]
        public async Task<IActionResult> GetByTournamentCategoryAndRound(int tournamentId, int categoryId, int round)
        {
            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.TournamentId == tournamentId
                        && m.Round == round
                        && m.HomeTeam.CategoryId == categoryId
                        && m.AwayTeam.CategoryId == categoryId)
                .OrderBy(m => m.Id)
                .Select(m => new MatchDto
                {
                    Id = m.Id,
                    Round = m.Round,
                    HomeTeam = m.HomeTeam.Name,
                    AwayTeam = m.AwayTeam.Name,
                    HomeScore = m.HomeScore,
                    AwayScore = m.AwayScore
                })
                .ToListAsync();

            if (!matches.Any()) return NotFound("No hay partidos para esa ronda y categoría");

            return Ok(new
            {
                TournamentId = tournamentId,
                CategoryId = categoryId,
                Round = round,
                Matches = matches
            });

        }


        //Get api/Matches/tournament/{tournamentId}/category/{categoryId}
        //Mejora del método GetByTournamentAndCategory para incluir el nombre de los equipos y devolver un objeto con la información del torneo, categoría y partidos
        //Mejora utíl, consultar todas las rondas de una categoría.
        [HttpGet("tournament/{tournamentId}/category/{categoryId}")]
        public async Task<IActionResult> GetByTournamentAndCategory(int tournamentId, int categoryId)
        {
            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.TournamentId == tournamentId
                            && m.HomeTeam.CategoryId == categoryId
                            && m.AwayTeam.CategoryId == categoryId)
                .OrderBy(m => m.Round)
                .ThenBy(m => m.Id)
                .Select(m => new MatchDto
                {
                    Id = m.Id,
                    Round = m.Round,
                    HomeTeam = m.HomeTeam.Name,
                    AwayTeam = m.AwayTeam.Name,
                    HomeScore = m.HomeScore,
                    AwayScore = m.AwayScore
                })
                .ToListAsync();

            if (!matches.Any())
                return NotFound("No hay partidos para ese torneo y categoría");

            return Ok(matches);
        }


        //Put api/Matches/{id}/result
        //Mejora del método updateResult para validar que los goles no sean negativos y devolver un mensaje más detallado al actualizar el resultado
        [HttpPut("{id}/result")]
        public async Task<IActionResult> UpdateResult(int id, UpdateMatchResultDto dto)
        {
            //Mejora del método updateResult
            if (dto.HomeScore < 0 || dto.AwayScore < 0)
                return BadRequest("Los goles no pueden ser negativos");
            //

            var match = await _context.Matches.FindAsync(id);

            if (match == null)
                return NotFound("Partido no encontrado");

            match.HomeScore = dto.HomeScore;
            match.AwayScore = dto.AwayScore;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Resultado actualizado",
                MatchId = match.Id,
                HomeScore = match.HomeScore,
                AwayScore = match.AwayScore
            });
        }



    }
}
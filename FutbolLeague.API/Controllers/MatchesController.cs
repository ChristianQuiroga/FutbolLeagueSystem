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

        //GET: api/Matches
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

        //Put api/Matches/{id}/result
        //
        [HttpPut("{id}/result")]
        public async Task<IActionResult> UpdateResult(int id, UpdateMatchResultDto dto)
        {
            //Mejora del método updateResult
            if(dto.HomeScore < 0 || dto.AwayScore < 0)
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
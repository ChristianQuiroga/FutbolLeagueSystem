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
                .Select(m => new MatchDto
                {
                   Id = m.id,
                    HomeTeam = m.HomeTeam.Name,
                    AwayTeam = m.AwayTeam.Name,
                    HomeScore=m.HomeScore,
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
    }
}

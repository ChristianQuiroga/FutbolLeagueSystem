using FutbolLeague.Application.DTOs;
using FutbolLeague.Domain;
using FutbolLeague.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FutbolLeague.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FixturesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FixturesController(AppDbContext context)
        {
            _context = context;
        }

        //Post
        [HttpPost("generate")]
        public async Task<IActionResult> Generate(GenerateFixtureDto dto)
        {
            var tournament = await _context.Tournaments
                .FirstOrDefaultAsync(t => t.Id == dto.TournamentId);

            if (tournament == null)
                return BadRequest("El torneo no existe");

            var teams = await _context.Teams
                .Where(t => t.TournamentId == dto.TournamentId)
                .OrderBy(t => t.Id)
                .ToListAsync();

            if (teams.Count < 2)
                return BadRequest("El torneo necesita al menos 2 equipos");

            var existingMatches = await _context.Matches
                .AnyAsync(m => m.TournamentId == dto.TournamentId);

            if (existingMatches)
                return BadRequest("Ese torneo ya tiene fixture generado");

            var matches = new List<Match>();

            for (int i = 0; i < teams.Count; i++)
            {
                for (int j = i + 1; j < teams.Count; j++)
                {
                    matches.Add(new Match
                    {
                        TournamentId = dto.TournamentId,
                        HomeTeamId = teams[i].Id,
                        AwayTeamId = teams[j].Id
                    });

                    if (tournament.FixtureFormat == FixtureFormat.DoubleRoundRobin)
                    {
                        matches.Add(new Match
                        {
                            TournamentId = dto.TournamentId,
                            HomeTeamId = teams[j].Id,
                            AwayTeamId = teams[i].Id
                        });
                    }
                }
            }

            _context.Matches.AddRange(matches);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                TournamentId = dto.TournamentId,
                TournamentName = tournament.Name,
                Format = tournament.FixtureFormat.ToString(),
                TeamsCount = teams.Count,
                MatchesGenerated = matches.Count
            });
        }
    }
}
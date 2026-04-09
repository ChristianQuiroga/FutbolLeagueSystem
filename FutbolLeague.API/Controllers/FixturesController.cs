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

            var teamList = teams.ToList();

            // Si es impar, agregamos un "bye"
            if (teamList.Count % 2 != 0)
                teamList.Add(null);

            int totalTeams = teamList.Count;
            int rounds = totalTeams - 1;
            int matchesPerRound = totalTeams / 2;

            for (int round = 1; round <= rounds; round++)
            {
                for (int matchIndex = 0; matchIndex < matchesPerRound; matchIndex++)
                {
                    var home = teamList[matchIndex];
                    var away = teamList[totalTeams - 1 - matchIndex];

                    if (home != null && away != null)
                    {
                        matches.Add(new Match
                        {
                            TournamentId = dto.TournamentId,
                            HomeTeamId = home.Id,
                            AwayTeamId = away.Id,
                            Round = round
                        });

                        if (tournament.FixtureFormat == FixtureFormat.DoubleRoundRobin)
                        {
                            matches.Add(new Match
                            {
                                TournamentId = dto.TournamentId,
                                HomeTeamId = away.Id,
                                AwayTeamId = home.Id,
                                Round = round + rounds
                            });
                        }
                    }
                }

                // Rotación de equipos (excepto el primero)
                var lastTeam = teamList[totalTeams - 1];
                teamList.RemoveAt(totalTeams - 1);
                teamList.Insert(1, lastTeam);
            }

            _context.Matches.AddRange(matches);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                TournamentId = dto.TournamentId,
                TournamentName = tournament.Name,
                Format = tournament.FixtureFormat.ToString(),
                TeamsCount = teams.Count,
                RoundsGenerated = tournament.FixtureFormat == FixtureFormat.DoubleRoundRobin ? rounds * 2 : rounds,
                MatchesGenerated = matches.Count
            });
        }
    }
}
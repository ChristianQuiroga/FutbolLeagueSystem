using FutbolLeague.Application.DTOs;
using FutbolLeague.Domain;
using FutbolLeague.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//Ctrol + M, O para colapsar todo el código y tener una vista general del controlador
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
        // Endpoint para generar el fixture de un torneo completo!
        [Obsolete("Este endpoint es obsoleto. Use /generate-by-category para generar por categoría.")]
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



        //Agregar nuevo endpoint para generar fixture por categoría
        //Post "api/fixtures/generate-by-category"
        [HttpPost("generate-by-category")]
        public async Task<IActionResult> GenerateByCategory(GenerateFixtureByCategoryDto dto)
        {
            var tournament = await _context.Tournaments
                .FirstOrDefaultAsync(t => t.Id == dto.TournamentId);

            if (tournament == null)
                return BadRequest("El torneo no existe");

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == dto.CategoryId);

            if (!categoryExists)
                return BadRequest("La categoría no existe");

            var teams = await _context.Teams
                .Where(t => t.TournamentId == dto.TournamentId && t.CategoryId == dto.CategoryId)
                .OrderBy(t => t.Id)
                .ToListAsync();

            if (teams.Count < 2)
                return BadRequest("La categoría necesita al menos 2 equipos para generar fixture");

            var teamIds = teams.Select(t => t.Id).ToList();

            var existingMatches = await _context.Matches
                .AnyAsync(m => m.TournamentId == dto.TournamentId &&
                               teamIds.Contains(m.HomeTeamId) &&
                               teamIds.Contains(m.AwayTeamId));

            if (existingMatches)
                return BadRequest("Esa categoría ya tiene fixture generado en este torneo");

            var matches = new List<Match>();
            var teamList = teams.ToList();

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
                            Round = round,
                            Status = MatchStatus.Pending
                        });

                        if (tournament.FixtureFormat == FixtureFormat.DoubleRoundRobin)
                        {
                            matches.Add(new Match
                            {
                                TournamentId = dto.TournamentId,
                                HomeTeamId = away.Id,
                                AwayTeamId = home.Id,
                                Round = round + rounds,
                                Status = MatchStatus.Pending
                            });
                        }
                    }
                }

                var lastTeam = teamList[totalTeams - 1];
                teamList.RemoveAt(totalTeams - 1);
                teamList.Insert(1, lastTeam);
            }

            _context.Matches.AddRange(matches);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                TournamentId = dto.TournamentId,
                CategoryId = dto.CategoryId,
                TournamentName = tournament.Name,
                Format = tournament.FixtureFormat.ToString(),
                TeamsCount = teams.Count,
                RoundsGenerated = tournament.FixtureFormat == FixtureFormat.DoubleRoundRobin ? rounds * 2 : rounds,
                MatchesGenerated = matches.Count
            });
        }


        // Endpoint para asignar fechas a los partidos de un torneo y categoría específicos
        //Post "api/fixtures/assign-dates"
        [HttpPost("assign-dates")]
        public async Task<IActionResult> AssignDates(AssignMatchDatesDto dto)
        {
            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Where(m => m.TournamentId == dto.TournamentId
                            && m.HomeTeam.CategoryId == dto.CategoryId)
                .OrderBy(m => m.Round)
                .ThenBy(m => m.Id)
                .ToListAsync();

            if (!matches.Any())
                return NotFound("No hay partidos para ese torneo y categoría");

            var rounds = matches
                .GroupBy(m => m.Round)
                .OrderBy(g => g.Key)
                .ToList();

            DateTime currentRoundDate = dto.StartDate.Date;

            foreach (var roundGroup in rounds)
            {
                DateTime currentDateTime = currentRoundDate + dto.MorningStartTime;

                foreach (var match in roundGroup)
                {
                    if (currentDateTime.TimeOfDay >= dto.LunchBreakStartTime &&
                        currentDateTime.TimeOfDay < dto.AfternoonStartTime)
                    {
                        currentDateTime = currentRoundDate + dto.AfternoonStartTime;
                    }

                    match.MatchDate = currentDateTime;
                    currentDateTime = currentDateTime.AddMinutes(dto.MinutesBetweenMatches);

                    if (currentDateTime.TimeOfDay >= dto.LunchBreakStartTime &&
                        currentDateTime.TimeOfDay < dto.AfternoonStartTime)
                    {
                        currentDateTime = currentRoundDate + dto.AfternoonStartTime;
                    }
                }

                currentRoundDate = currentRoundDate.AddDays(7);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Fechas asignadas correctamente",
                TournamentId = dto.TournamentId,
                CategoryId = dto.CategoryId,
                TotalMatches = matches.Count
            });
        }


        //Post "api/fixtures/assign-fields"
        // Endpoint para asignar canchas a los partidos de un torneo y categoría específicos
        [HttpPost("assign-fields")]
        public async Task<IActionResult> AssignFields(int tournamentId, int categoryId)
        {
            var fields = await _context.Fields.ToListAsync();

            if (!fields.Any())
                return BadRequest("No hay canchas cargadas");

            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Where(m => m.TournamentId == tournamentId &&
                            m.HomeTeam.CategoryId == categoryId)
                .OrderBy(m => m.Round)
                .ThenBy(m => m.MatchDate)
                .ToListAsync();

            if (!matches.Any())
                return NotFound("No hay partidos");

            var groupedByRound = matches
                .GroupBy(m => m.Round)
                .ToList();

            foreach (var round in groupedByRound)
            {
                int fieldIndex = 0;

                foreach (var match in round)
                {
                    //match.FieldId = fields[fieldIndex].Id
                    // Antes de asignar, verificamos que no haya otro partido en la misma fecha con la misma cancha
                    var field = fields[fieldIndex];

                    var conflict = matches.Any(m =>
                        m.Id != match.Id &&
                        m.FieldId == field.Id &&
                        m.MatchDate == match.MatchDate);

                    if (!conflict)
                    {
                        match.FieldId = field.Id;
                    }
                    // Si hay conflicto, se deja sin asignar para que el administrador lo resuelva manualmente

                    fieldIndex++;

                    if (fieldIndex >= fields.Count)
                        fieldIndex = 0;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Canchas asignadas automáticamente",
                MatchesUpdated = matches.Count
            });
        }
    }
}
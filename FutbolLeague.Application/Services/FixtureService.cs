using FutbolLeague.Application.DTOs;
using FutbolLeague.Domain;
using FutbolLeague.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FutbolLeague.Application.Services
{
    public class FixtureService : IFixtureService
    {
        private readonly AppDbContext _context;

        public FixtureService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> GenerateByCategoryAsync(GenerateFixtureByCategoryDto dto)
        {
            var tournament = await _context.Tournaments
                .FirstOrDefaultAsync(t => t.Id == dto.TournamentId);

            if (tournament == null)
                throw new Exception("El torneo no existe");

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == dto.CategoryId);

            if (!categoryExists)
                throw new Exception("La categoría no existe");

            var teams = await _context.Teams
                .Where(t => t.TournamentId == dto.TournamentId && t.CategoryId == dto.CategoryId)
                .OrderBy(t => t.Id)
                .ToListAsync();

            if (teams.Count < 2)
                throw new Exception("La categoría necesita al menos 2 equipos para generar fixture");

            var teamIds = teams.Select(t => t.Id).ToList();

            var existingMatches = await _context.Matches
                .Where(m => m.TournamentId == dto.TournamentId &&
                            teamIds.Contains(m.HomeTeamId) &&
                            teamIds.Contains(m.AwayTeamId))
                .ToListAsync();

            if (existingMatches.Any(m => m.Status == MatchStatus.Played))
                throw new Exception("No se puede regenerar el fixture porque ya hay partidos jugados en esta categoría");

            if (existingMatches.Any())
                throw new Exception("Esa categoría ya tiene fixture generado en este torneo");

            var matches = new List<Match>();
            var teamList = teams.Cast<Team?>().ToList();

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

            return new
            {
                TournamentId = dto.TournamentId,
                CategoryId = dto.CategoryId,
                TournamentName = tournament.Name,
                Format = tournament.FixtureFormat.ToString(),
                TeamsCount = teams.Count,
                RoundsGenerated = tournament.FixtureFormat == FixtureFormat.DoubleRoundRobin ? rounds * 2 : rounds,
                MatchesGenerated = matches.Count
            };
        }

        public async Task<object> AssignDatesAsync(AssignMatchDatesDto dto)
        {
            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Where(m => m.TournamentId == dto.TournamentId &&
                            m.HomeTeam.CategoryId == dto.CategoryId)
                .OrderBy(m => m.Round)
                .ThenBy(m => m.Id)
                .ToListAsync();

            if (!matches.Any())
                throw new Exception("No hay partidos para ese torneo y categoría");

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

            return new
            {
                Message = "Fechas asignadas correctamente",
                TournamentId = dto.TournamentId,
                CategoryId = dto.CategoryId,
                TotalMatches = matches.Count
            };
        }

        public async Task<object> AssignFieldsAsync(int tournamentId, int categoryId)
        {
            var teams = await _context.Teams
                .Where(t => t.TournamentId == tournamentId && t.CategoryId == categoryId)
                .ToListAsync();

            if (!teams.Any())
                throw new Exception("No hay equipos para ese torneo y categoría");

            var teamIds = teams.Select(t => t.Id).ToList();

            var matches = await _context.Matches
                .Where(m => m.TournamentId == tournamentId &&
                            teamIds.Contains(m.HomeTeamId) &&
                            teamIds.Contains(m.AwayTeamId))
                .OrderBy(m => m.Round)
                .ThenBy(m => m.MatchDate)
                .ToListAsync();

            if (!matches.Any())
                throw new Exception("No hay partidos para asignar canchas");

            var fields = await _context.Fields.ToListAsync();

            if (!fields.Any())
                throw new Exception("No hay canchas registradas");

            int fieldIndex = 0;

            foreach (var match in matches)
            {
                match.FieldId = fields[fieldIndex].Id;

                fieldIndex++;
                if (fieldIndex >= fields.Count)
                    fieldIndex = 0;
            }

            await _context.SaveChangesAsync();

            return new
            {
                Message = "Canchas asignadas correctamente",
                TournamentId = tournamentId,
                CategoryId = categoryId,
                TotalMatches = matches.Count
            };
        }

        public async Task<object> DeleteFixtureByCategoryAsync(int tournamentId, int categoryId)
        {
            var teams = await _context.Teams
                .Where(t => t.TournamentId == tournamentId && t.CategoryId == categoryId)
                .ToListAsync();

            if (!teams.Any())
                throw new Exception("No hay equipos para ese torneo y categoría");

            var teamIds = teams.Select(t => t.Id).ToList();

            var matches = await _context.Matches
                .Where(m => m.TournamentId == tournamentId &&
                            teamIds.Contains(m.HomeTeamId) &&
                            teamIds.Contains(m.AwayTeamId))
                .ToListAsync();

            if (!matches.Any())
                throw new Exception("No hay fixture generado para ese torneo y categoría");

            if (matches.Any(m => m.Status == MatchStatus.Played))
                throw new Exception("No se puede eliminar el fixture porque ya hay partidos jugados en esta categoría");

            _context.Matches.RemoveRange(matches);
            await _context.SaveChangesAsync();

            return new
            {
                Message = "Fixture eliminado correctamente",
                TournamentId = tournamentId,
                CategoryId = categoryId,
                MatchesDeleted = matches.Count
            };
        }
    }
}
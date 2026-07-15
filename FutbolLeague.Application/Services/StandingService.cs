using FutbolLeague.Application.DTOs;
using FutbolLeague.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace FutbolLeague.Application.Services
{
    public class StandingService : IStandingService
    {
        private readonly AppDbContext _context;

        public StandingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<StandingDto>> GetStandingsByTournamentAsync(int tournamentId)
        {
            var teams = await _context.Teams
                .Where(t => t.TournamentId == tournamentId)
                .ToListAsync();

            if (!teams.Any())
                return new List<StandingDto>();

            var matches = await _context.Matches
                .Where(m => m.TournamentId == tournamentId &&
                            m.Status == Domain.MatchStatus.Played && // Only consider matches that have been played
                            m.HomeScore.HasValue &&
                            m.AwayScore.HasValue)
                .ToListAsync();

            return BuildStandings(teams, matches);
        }

        public async Task<List<StandingDto>> GetStandingsByTournamentAndCategoryAsync(int tournamentId, int categoryId)
        {
            var teams = await _context.Teams
                .Where(t => t.TournamentId == tournamentId && t.CategoryId == categoryId)
                .ToListAsync();

            if (!teams.Any())
                return new List<StandingDto>();

            var teamIds = teams.Select(t => t.Id).ToList();

            var matches = await _context.Matches
                .Where(m => m.TournamentId == tournamentId &&
                            m.Status == Domain.MatchStatus.Played &&
                            m.HomeScore.HasValue &&
                            m.AwayScore.HasValue &&
                            teamIds.Contains(m.HomeTeamId) &&
                            teamIds.Contains(m.AwayTeamId))
                .ToListAsync();

            return BuildStandings(teams, matches);
        }

        private List<StandingDto> BuildStandings(List<Domain.Team> teams, List<Domain.Match> matches)
        {
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

            return orderedStandings;
        }
    }
}
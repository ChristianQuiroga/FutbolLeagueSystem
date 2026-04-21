using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Application.DTOs
{
    internal class TournamentSummaryDto
    {
        public int TournamentId { get; set; }
        public int CategoryId { get; set; }

        public int TotalTeams { get; set; }
        public int TotalMatches { get; set; }
        public int PlayedMatches { get; set; }
        public int PendingMatches { get; set; }

        public List<MatchDto> NextMatches { get; set; } // Upcoming matches in the tournament
        public List<MatchDto> LastResults { get; set; } // Recently played matches with results

        public List<StandingDto> Standings { get; set; } // Current standings of teams in the tournament

    }
}

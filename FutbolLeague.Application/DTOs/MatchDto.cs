using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Application.DTOs
{
    public class MatchDto
    {
        public int Id { get; set; }

        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }

        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }

    }
}

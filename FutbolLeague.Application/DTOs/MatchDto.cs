using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Application.DTOs
{
    public class MatchDto
    {
        public int Id { get; set; }
        public int Round { get; set; } // Vamos a guardar al fecha del partido, para poder ordenar los partidos por fecha

        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }

        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }

        public string Status { get; set; } // Pendiente, Jugado, etc.

        public DateTime? MatchDate { get; set; } // Fecha del partido, para poder ordenar los partidos por fecha

        public string? Field { get; set; } // Campo donde se juega el partido

    }
}

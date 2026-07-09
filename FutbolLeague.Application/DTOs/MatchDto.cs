using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Application.DTOs
{
    public class MatchDto
    {
        public int Id { get; set; }


        public int TournamentId { get; set; }
        public string TournamentName { get; set; } = string.Empty;
        public string TournamentStatus { get; set; } = string.Empty;


        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;


        public int Round { get; set; } // Vamos a guardar al fecha del partido, para poder ordenar los partidos por fecha


        public int HomeTeamId { get; set; }
        public string HomeTeam { get; set; } = string.Empty;


        public int AwayTeamId { get; set; }
        public string AwayTeam { get; set; } = string.Empty;

        
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }


        public string Status { get; set; } = string.Empty; // Pendiente, Jugado, etc.

        
        public DateTime? MatchDate { get; set; } // Fecha del partido, para poder ordenar los partidos por fecha


        public int? FieldId { get; set; } 
        public string? Field { get; set; } // Campo donde se juega el partido

    }
}

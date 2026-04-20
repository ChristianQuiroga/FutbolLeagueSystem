using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Domain
{
    public class Match
    {
        public int Id { get; set; }

        //relationships con Tournament, Team
        public int TournamentId { get; set; }
        public Tournament? Tournament { get; set; }

        public int HomeTeamId { get; set; }
        public Team? HomeTeam { get; set; }  

        public int AwayTeamId { get; set; }
        public Team? AwayTeam { get; set; }

        public int Round { get; set; } // Vamos a guardar la Fecha del partido, para poder ordenar los partidos por fecha

        public int? HomeScore {  get; set; }
        public int? AwayScore { get; set; }

        public MatchStatus Status { get; set; } // Estado del partido, para saber si se ha jugado o no

        public DateTime? MatchDate { get; set; } // Fecha del partido, para poder ordenar los partidos por fecha

        
        // Relación con Field
        public int? FieldId { get; set; } // Id del campo donde se juega el partido
        public Field? Field { get; set; } // Campo donde se juega el partido

    }
}

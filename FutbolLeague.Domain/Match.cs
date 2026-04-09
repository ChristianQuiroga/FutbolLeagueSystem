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

        public int? HomeScore {  get; set; }
        public int? AwayScore { get; set; }

        public int Round { get; set; } // Vamos a guardar la fecha del partido, para poder ordenar los partidos por fecha

    }
}

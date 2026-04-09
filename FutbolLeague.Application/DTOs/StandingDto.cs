using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Application.DTOs
{
    public class StandingDto
    {
        public int Position { get; set; } //Agregar posición a la tabla

        public int TeamId { get; set; } // Id del equipo
        public string TeamName { get; set; } // Nombre del equipo

        public int Played { get; set; } // Partidos jugados
        public int Won { get; set; } // Victorias
        public int Drawn { get; set; } // Empates
        public int Lost { get; set; } // Derrotas

        public int GoalsFor { get; set; } // Goles a favor
        public int GoalsAgainst { get; set; } // Goles en contra
        //public int GoalDifference => GoalsFor - GoalsAgainst; // Diferencia de goles
        public int GoalDifference {  get; set; }

        public int Points {  get; set; } // Puntos totales (3 por victoria, 1 por empate, 0 por derrota)
    }
}

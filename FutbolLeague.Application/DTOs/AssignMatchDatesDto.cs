using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Application.DTOs
{
    public class AssignMatchDatesDto
    {
        public int TournamentId { get; set; }
        public int CategoryId { get; set; }

        public DateTime StartDate { get; set; }

        public TimeSpan MorningStartTime { get; set; } // Hora de inicio para el primer partido de la mañana Ej: 09:00:00
        public TimeSpan LunchBreakStartTime { get; set; } // Hora de inicio del descanso para almuerzo Ej: 12:00:00
        public TimeSpan AfternoonStartTime { get; set; } // Hora de inicio para el primer partido de la tarde Ej: 14:00:00

        public int MinutesBetweenMatches { get; set; } // Minutos entre partidos Ej: 120
    }
}

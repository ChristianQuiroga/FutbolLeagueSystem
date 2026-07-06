using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Domain
{
    // Enum representing the status of a tournament
    public enum TournamentStatus
    {
        Pending = 1, // El torneo está programado pero aún no ha comenzado
        InProgress = 2, // El torneo está en curso
        Finished = 3, // El torneo ha finalizado
    }
}

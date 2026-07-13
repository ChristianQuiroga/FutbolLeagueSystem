using System;
using System.Collections.Generic;
using System.Text;
using FutbolLeague.Application.DTOs;

namespace FutbolLeague.Application.Services
{
    public interface IMatchService
    {
        //Interface for managing matches, including creating, updating, and retrieving match information.
        //Este es el contrato para el servicio de gestión de partidos, que incluye la creación, actualización y recuperación de información de los partidos.

        Task<object> UpdateResultAsync(int matchId, UpdateMatchResultDto dto); // Update home and away scores
        Task<object> UpdateDateAsync(int matchId, UpdateMatchDateDto dto); // Update the date of the match
        Task<object> UpdateStatusAsync(int matchId, string status); // "Scheduled", "InProgress", "Completed"
        Task<object> UpdateFieldAsync(int matchId, int fieldId);
    }
}

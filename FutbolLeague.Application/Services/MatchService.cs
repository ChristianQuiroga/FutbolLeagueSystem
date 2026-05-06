using FutbolLeague.Application.DTOs;
using FutbolLeague.Application.Exceptions;
using FutbolLeague.Domain;
using FutbolLeague.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FutbolLeague.Application.Services
{
    public class MatchService : IMatchService
    {
        private readonly AppDbContext _context;

        public MatchService(AppDbContext context)
        {
            _context = context;
        }

        // Implementación de los métodos para actualizar el resultado, la fecha y el estado de un partido.
        public async Task<object> UpdateResultAsync(int matchId, UpdateMatchResultDto dto)
        {
            var match = await _context.Matches.FindAsync(matchId); // Busca el partido por su ID

            if (match == null)
                throw new NotFoundException("Partido no encontrado"); // Si no se encuentra el partido, lanza una excepción

            if (dto.HomeScore < 0 || dto.AwayScore < 0) 
                throw new BusinessException("Los goles no pueden ser negativos"); // Validación para asegurar que los goles no sean negativos

            match.HomeScore = dto.HomeScore; // Actualiza el resultado del equipo local
            match.AwayScore = dto.AwayScore; // Actualiza el resultado del equipo visitante
            match.Status = MatchStatus.Played; // Cambia el estado del partido a "Jugado" después de actualizar el resultado

            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos

            return new // Devuelve un objeto con un mensaje de éxito y los detalles del partido actualizado
            {
                Message = "Resultado actualizado correctamente",
                MatchId = match.Id,
                HomeScore = match.HomeScore,
                AwayScore = match.AwayScore,
                Status = match.Status.ToString()
            };
        }

        public async Task<object> UpdateDateAsync(int matchId, UpdateMatchDateDto dto)
        {
            var match = await _context.Matches.FindAsync(matchId);

            if (match == null)
                throw new NotFoundException("Partido no encontrado");

            match.MatchDate = dto.MatchDate;

            await _context.SaveChangesAsync();

            return new
            {
                Message = "Fecha actualizada correctamente",
                MatchId = match.Id,
                MatchDate = match.MatchDate
            };
        }

        public async Task<object> UpdateStatusAsync(int matchId, string status)
        {
            var match = await _context.Matches.FindAsync(matchId);

            if (match == null)
                throw new NotFoundException("Partido no encontrado");

            if (!Enum.TryParse<MatchStatus>(status, true, out var parsedStatus))
                throw new BusinessException("Estado inválido");

            match.Status = parsedStatus;

            await _context.SaveChangesAsync();

            return new
            {
                Message = "Estado actualizado correctamente",
                MatchId = match.Id,
                Status = match.Status.ToString()
            };
        }
    }
}
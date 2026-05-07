using FutbolLeague.Application.DTOs;

namespace FutbolLeague.Application.Services
{
    /*
    Pasar de esto:
    TournamentsController con lógica
    a esto:
    TournamentsController → ITournamentService → TournamentService

    Vamos a mover principalmente:

    crear torneo
    listar torneos
    resumen del torneo por categoría
    */

    public interface ITournamentService
    {
        Task<List<TournamentDto>> GetAllAsync(); // Listar torneos
        Task<TournamentDto> CreateAsync(CreateTournamentDto dto); // Crear torneo
        Task<object> GetSummaryAsync(int tournamentId, int categoryId); // Resumen del torneo por categoría (puede ser un DTO específico en lugar de object)
    }
}

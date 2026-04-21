using FutbolLeague.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Application.Services
{
    public interface IStandingService
    {
        Task<List<StandingDto>> GetStandingsByTournamentAsync(int tournamentId); // Get standings for a specific tournament
        Task<List<StandingDto>> GetStandingsByTournamentAndCategoryAsync(int tournamentId, int categoryId); // Get standings for a specific tournament and category

    }
}

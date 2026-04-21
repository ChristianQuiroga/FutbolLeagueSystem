using FutbolLeague.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Application.Services
{
    public interface IFixtureService
    {
        Task<object> GenerateByCategoryAsync(GenerateFixtureByCategoryDto dto); // Consider changing the return type to a more specific type instead of object
        Task<object> AssignDatesAsync(AssignMatchDatesDto dto);
        Task<object> AssignFieldsAsync(int tournamentId, int categoryId);
        Task<object> DeleteFixtureByCategoryAsync(int tournamentId, int categoryId);
    }
}

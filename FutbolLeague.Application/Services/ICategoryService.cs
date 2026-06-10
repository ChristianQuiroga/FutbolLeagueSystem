using FutbolLeague.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Application.Services
{
    // La interface EXIGE que existan estos 3 métodos
    // pero no dice CÓMO funcionan — eso lo define la clase que la implemente
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync();
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
        Task DeleteAsync(int id);
    }
}

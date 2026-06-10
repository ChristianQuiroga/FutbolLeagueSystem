using FutbolLeague.Application.DTOs;
using FutbolLeague.Application.Exceptions;
using FutbolLeague.Domain;
using FutbolLeague.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FutbolLeague.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsActive = c.IsActive
                })
                .ToListAsync();
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var exists = await _context.Categories
                .AnyAsync(c => c.Name == dto.Name && c.IsActive);

            if (exists)
                throw new ConflictException("Ya existe una categoría activa con ese nombre");

            var category = new Category
            {
                Name = dto.Name,
                IsActive = true
            };

            _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                IsActive = category.IsActive
            };
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                throw new NotFoundException("La categoría no existe");

            var hasTeams = await _context.Teams
                .AnyAsync(t => t.CategoryId == id);

            if (hasTeams)
                throw new ConflictException("No se puede eliminar la categoría porque tiene equipos asociados");

            category.IsActive = false;

            await _context.SaveChangesAsync();
        }
    }
}
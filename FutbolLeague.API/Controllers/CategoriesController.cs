
using FutbolLeague.Domain;
using FutbolLeague.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using FutbolLeague.Application.DTOs;


namespace FutbolLeague.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Inyectamos el contexto de la base de datos a través del constructor
        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }


        // GET: api/Categories
        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var categories = await _context.Categories.ToListAsync();
        //    return Ok(categories);
        //}
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            return Ok(categories);
        }

        // POST: api/Categories
        //[HttpPost]
        //public async Task<IActionResult> Create(Category category)
        //{
        //    if(string.IsNullOrEmpty(category.Name))
        //    {
        //        return BadRequest("El nombre de la Category es obligatorio.");
        //    }
        //    _context.Categories.Add(category);
        //    await _context.SaveChangesAsync();

        //    return Ok(category);
        //}
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
                return BadRequest("El nombre de la Category es obligatorio.");

            //La categoria no puede estar duplicada con el mismo nombre.
            if(await _context.Categories
                .AnyAsync(c => c.Name == dto.Name)) 
                return BadRequest("Ya existe esa misma Categoría, esta duplicada");
            

            var category = new Category
            {
                Name = dto.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            });
        }
    }
}

using FutbolLeague.Domain;
using FutbolLeague.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }


        // POST: api/Categories
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if(string.IsNullOrEmpty(category.Name))
            {
                return BadRequest("El nombre de la Category es obligatorio.");
            }
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(category);
        }
    }
}
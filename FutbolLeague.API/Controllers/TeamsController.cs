using FutbolLeague.Domain;
using FutbolLeague.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FutbolLeague.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Inyectamos el contexto de la base de datos a través del constructor
        public TeamsController(AppDbContext context)
        {
            _context = context;
        }


        // GET: api/Teams
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teams = await _context.Teams
                .Include(t => t.Category)
                .ToListAsync();

            return Ok(teams);
        }


        // POST: api/Teams
        [HttpPost]
        public async Task<IActionResult> Create(Team team)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == team.CategoryId);

            // Validamos que la categoría exista
            if (!categoryExists)
                return BadRequest("La categoría no existe");
            
            // Validamos que el nombre del equipo no esté vacío
            if (string.IsNullOrEmpty(team.Name))
                return BadRequest("El nombre del equipo es obligatorio");

            // El nombre del equipo debe tener al menos 3 caracteres

            // El nombre del equipo no puede repetirse dentro de la misma categoría
            if(await _context.Teams
                .AnyAsync(t => t.Name == team.Name && t.CategoryId == team.CategoryId))
                return BadRequest("Ya existe un equipo con ese nombre en la misma categoría");


            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return Ok(team);
        }
    }
}
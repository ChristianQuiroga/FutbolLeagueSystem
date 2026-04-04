using FutbolLeague.Application.DTOs;
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
        // Este método obtiene todos los equipos, incluyendo el nombre de su categoría, con DTOs
        //Dtos: Data Transfer Objects, son objetos que se utilizan para transferir datos entre capas de una aplicación, especialmente entre la capa de presentación y la capa de negocio o de acceso a datos. Los DTOs suelen ser clases simples que contienen propiedades para representar los datos que se desean transferir, sin incluir lógica de negocio ni métodos complejos. El uso de DTOs ayuda a desacoplar las diferentes capas de la aplicación, mejorar la seguridad al exponer solo los datos necesarios y facilitar la serialización y deserialización de datos en formatos como JSON o XML.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teams = await _context.Teams
                .Include(t => t.Category)
                .Select(t => new TeamDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category.Name
                })
                .ToListAsync();
            return Ok(teams);
        }


        // POST: api/Teams
        [HttpPost]
        public async Task<IActionResult> Create(TeamDto teamDto)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == teamDto.CategoryId);
            
            // Validamos que la categoría exista
            if (!categoryExists)
                return BadRequest("La categoría no existe");
            
            // Validamos que el nombre del equipo no esté vacío
            if (string.IsNullOrEmpty(teamDto.Name))
                return BadRequest("El nombre del equipo es obligatorio");

            // El nombre del equipo debe tener al menos 3 caracteres

            // El nombre del equipo no puede repetirse dentro de la misma categoría
            if (await _context.Teams
                .AnyAsync(t => t.Name == teamDto.Name && t.CategoryId == teamDto.CategoryId))
                return BadRequest("Ya existe un equipo con ese nombre en la misma categoría");

            var team = new Team
            {
                Name = teamDto.Name,
                CategoryId = teamDto.CategoryId
            };

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return Ok(new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                CategoryId = team.CategoryId,
                //CategoryName = (await _context.Categories.FindAsync(team.CategoryId))?.Name
                CategoryName = ""
            });

        }
    }
}
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
                .Include(t => t.Tournament)
                .Select(t => new TeamDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category.Name,
                    TournamentId = t.TournamentId,
                    TournamentName = t.Tournament.Name
                })
                .ToListAsync();

            return Ok(teams);
        }




        // POST: api/Teams
        [HttpPost]
        public async Task<IActionResult> Create(CreateTeamDto dto)
        {
            //Console.WriteLine($"Name: {dto.Name}");
            //Console.WriteLine($"CategoriaId: {dto.CategoryId}");
            //Console.WriteLine($"TournamentId: {dto.TournamentId}");


            // Validamos que el nombre del equipo no esté vacío
            if (string.IsNullOrEmpty(dto.Name))
                return BadRequest("El nombre del equipo es obligatorio");

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == dto.CategoryId);

            // Validamos que la categoría exista
            if (!categoryExists)
                return BadRequest("La categoría no existe");


            // El nombre del equipo debe tener al menos 3 caracteres

            // El nombre del equipo no puede repetirse dentro de la misma categoría
            if (await _context.Teams
                .AnyAsync(t => t.Name == dto.Name && t.CategoryId == dto.CategoryId))
                return BadRequest("Ya existe un equipo con ese nombre en la misma categoría");

            var tournamentExists = await _context.Tournaments
                .AnyAsync(t => t.Id == dto.TournamentId);

            if (!tournamentExists)
                return BadRequest("El torneo no existe");


            var team = new Team
            {
                Name = dto.Name,
                CategoryId = dto.CategoryId,
                TournamentId = dto.TournamentId
            };

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            var result = await _context.Teams
                .Include(t => t.Category)
                .Include(t => t.Tournament)
                .Where(t => t.Id == team.Id)
                .Select(t => new TeamDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category.Name,
                    TournamentId = t.TournamentId,
                    TournamentName = t.Tournament.Name
                })
            .FirstAsync();

            return Ok(result);

        }
    }
}
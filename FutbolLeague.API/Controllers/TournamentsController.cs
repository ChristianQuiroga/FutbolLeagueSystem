using FutbolLeague.Application.DTOs;
using FutbolLeague.Domain;
using FutbolLeague.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//Ctrol + M, O para colapsar todo el código y tener una vista general del controlador
namespace FutbolLeague.API.Controllers
{
    /// Controller for managing tournaments in the football league application
    [ApiController]
    [Route("api/[controller]")]
    public class TournamentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TournamentsController(AppDbContext context)
        {
            _context = context;
        }


        // GET: api/tournaments
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tournaments = await _context.Tournaments
                .Select(t => new TournamentDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    FixtureFormat = (int)t.FixtureFormat,
                    FixtureFormatName = t.FixtureFormat.ToString()
                })
                .ToListAsync();

            return Ok(tournaments);
        }


        // POST: api/tournaments
        [HttpPost]
        public async Task<IActionResult> Create(CreateTournamentDto dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
                return BadRequest("El nombre es obligatorio");

            if (!Enum.IsDefined(typeof(FixtureFormat), dto.FixtureFormat))
                return BadRequest("Formato de fixture inválido");

            // Verificar si ya existe un torneo con el mismo nombre (ignorando mayúsculas)
            var exists = await _context.Tournaments
                .AnyAsync(t => t.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                return BadRequest("Ya existe un torneo con ese nombre");
            //

            var tournament = new Tournament
            {
                Name = dto.Name,
                FixtureFormat = (FixtureFormat)dto.FixtureFormat
            };

            _context.Tournaments.Add(tournament);
            await _context.SaveChangesAsync();

            return Ok(new TournamentDto
            {
                Id = tournament.Id,
                Name = tournament.Name,
                FixtureFormat = (int)tournament.FixtureFormat,
                FixtureFormatName = tournament.FixtureFormat.ToString()
            });
        }
    }
}
using FutbolLeague.Application.Services;
using Microsoft.AspNetCore.Mvc;

//Ctrol + M + O para colapsar todo el código
namespace FutbolLeague.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StandingsController : ControllerBase
    {
        private readonly IStandingService _standingService;

        public StandingsController(IStandingService standingService)
        {
            _standingService = standingService;
        }

        // GET: api/Standings/tournament/5
        // Obtener la tabla de posiciones para un torneo específico
        [HttpGet("tournament/{tournamentId}")]
        public async Task<IActionResult> GetByTournament(int tournamentId)
        {
            var standings = await _standingService.GetStandingsByTournamentAsync(tournamentId);

            if (!standings.Any())
                return NotFound("No hay equipos o partidos para ese torneo");

            return Ok(standings);
        }

        // GET: api/Standings/tournament/5/category/3
        // Obtener la tabla de posiciones para un torneo específico y una categoría específica
        [HttpGet("tournament/{tournamentId}/category/{categoryId}")]
        public async Task<IActionResult> GetByTournamentAndCategory(int tournamentId, int categoryId)
        {
            var standings = await _standingService.GetStandingsByTournamentAndCategoryAsync(tournamentId, categoryId);

            if (!standings.Any())
                return NotFound("No hay equipos o partidos para ese torneo y categoría");

            return Ok(standings);
        }
    }
}


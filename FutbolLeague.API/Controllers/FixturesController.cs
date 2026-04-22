using FutbolLeague.Application.DTOs;
using FutbolLeague.Application.Services;
using Microsoft.AspNetCore.Mvc;
//Ctrol + M + O para organizar los using
//ctrol + M + L para colapsar todo el código
//Ctrol + M + P para expandir todo el código
//Ctrol + K + D para organizar el código

namespace FutbolLeague.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FixturesController : ControllerBase
    {
        private readonly IFixtureService _fixtureService;
        private readonly ILogger<FixturesController> _logger; // Inyección de logger

        public FixturesController(IFixtureService fixtureService, ILogger<FixturesController> logger)
        {
            _fixtureService = fixtureService;
            _logger = logger;
        }

        // POST: api/fixtures/generate-by-category
        // Body: { "tournamentId": 1, "categoryId": 2 }
        [HttpPost("generate-by-category")]
        public async Task<IActionResult> GenerateByCategory(GenerateFixtureByCategoryDto dto)
        {
            try
            {
                _logger.LogInformation(
                    "Generando fixture para TournamentId={TournamentId}, CategoryId={CategoryId}",
                    dto.TournamentId,
                    dto.CategoryId);

                var result = await _fixtureService.GenerateByCategoryAsync(dto);

                _logger.LogInformation(
                    "Fixture generado correctamente para TournamentId={TournamentId}, CategoryId={CategoryId}",
                    dto.TournamentId,
                    dto.CategoryId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al generar fixture para TournamentId={TournamentId}, CategoryId={CategoryId}",
                    dto.TournamentId,
                    dto.CategoryId);

                return BadRequest(ex.Message);
            }
        }


        // POST: api/fixtures/assign-dates
        // Body: { "tournamentId": 1, "categoryId": 2, "startDate": "2024-07-01T10:00:00", "endDate": "2024-07-31T18:00:00" }
        [HttpPost("assign-dates")]
        public async Task<IActionResult> AssignDates(AssignMatchDatesDto dto)
        {
            try
            {
                _logger.LogInformation(
                    "Asignando fechas para TournamentId={TournamentId}, CategoryId={CategoryId}",
                    dto.TournamentId,
                    dto.CategoryId);

                var result = await _fixtureService.AssignDatesAsync(dto);

                _logger.LogInformation(
                    "Fechas asignadas correctamente para TournamentId={TournamentId}, CategoryId={CategoryId}",
                    dto.TournamentId,
                    dto.CategoryId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al asignar fechas para TournamentId={TournamentId}, CategoryId={CategoryId}",
                    dto.TournamentId,
                    dto.CategoryId);

                return BadRequest(ex.Message);
            }
        }


        // DELETE: api/fixtures/tournament/1/category/2
        // Elimina el fixture completo para un torneo y categoría específicos
        [HttpDelete("tournament/{tournamentId}/category/{categoryId}")]
        public async Task<IActionResult> DeleteFixtureByCategory(int tournamentId, int categoryId)
        {
            try
            {
                _logger.LogInformation(
                    "Eliminando fixture para TournamentId={TournamentId}, CategoryId={CategoryId}",
                    tournamentId,
                    categoryId);

                var result = await _fixtureService.DeleteFixtureByCategoryAsync(tournamentId, categoryId);

                _logger.LogInformation(
                    "Fixture eliminado correctamente para TournamentId={TournamentId}, CategoryId={CategoryId}",
                    tournamentId,
                    categoryId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al eliminar fixture para TournamentId={TournamentId}, CategoryId={CategoryId}",
                    tournamentId,
                    categoryId);

                return BadRequest(ex.Message);
            }
        }


        // POST: api/fixtures/assign-fields
        // Body: { "tournamentId": 1, "categoryId": 2 }
        [HttpPost("assign-fields")]
        public async Task<IActionResult> AssignFields(int tournamentId, int categoryId)
        {
            try
            {
                _logger.LogInformation(
                    "Asignando canchas para TournamentId={TournamentId}, CategoryId={CategoryId}",
                    tournamentId,
                    categoryId);

                var result = await _fixtureService.AssignFieldsAsync(tournamentId, categoryId);

                _logger.LogInformation(
                    "Canchas asignadas correctamente para TournamentId={TournamentId}, CategoryId={CategoryId}",
                    tournamentId,
                    categoryId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al asignar canchas para TournamentId={TournamentId}, CategoryId={CategoryId}",
                    tournamentId,
                    categoryId);

                return BadRequest(ex.Message);
            }
        }
    }
}
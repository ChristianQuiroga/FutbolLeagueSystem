using FutbolLeague.Application.DTOs;
using FutbolLeague.Application.Services;
using Microsoft.AspNetCore.Mvc;

//Ctrol + M + O para colapsar todo el código
namespace FutbolLeague.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FixturesController : ControllerBase
    {
        private readonly IFixtureService _fixtureService;

        public FixturesController(IFixtureService fixtureService)
        {
            _fixtureService = fixtureService;
        }

        // Post "api/fixtures/generate"
        // Endpoint para generar el fixture de un torneo completo (obsoleto, se recomienda usar el de categoría)
        [HttpPost("generate-by-category")]
        public async Task<IActionResult> GenerateByCategory(GenerateFixtureByCategoryDto dto)
        {
            try
            {
                var result = await _fixtureService.GenerateByCategoryAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // Post "api/fixtures/assign-dates"
        // Endpoint para asignar fechas a los partidos de un torneo y categoría específicos
        [HttpPost("assign-dates")]
        public async Task<IActionResult> AssignDates(AssignMatchDatesDto dto)
        {
            try
            {
                var result = await _fixtureService.AssignDatesAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // Post "api/fixtures/assign-fields"
        // Endpoint para asignar canchas a los partidos de un torneo y categoría específicos
        [HttpPost("assign-fields")]
        public async Task<IActionResult> AssignFields(int tournamentId, int categoryId)
        {
            try
            {
                var result = await _fixtureService.AssignFieldsAsync(tournamentId, categoryId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // Post "api/fixtures/assign-fields"
        // Endpoint para asignar canchas a los partidos de un torneo y categoría específicos
        [HttpDelete("tournament/{tournamentId}/category/{categoryId}")]
        public async Task<IActionResult> DeleteFixtureByCategory(int tournamentId, int categoryId)
        {
            try
            {
                var result = await _fixtureService.DeleteFixtureByCategoryAsync(tournamentId, categoryId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

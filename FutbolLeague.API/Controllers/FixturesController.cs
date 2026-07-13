using FutbolLeague.Application.DTOs;
using FutbolLeague.Application.Services;
using Microsoft.AspNetCore.Authorization;
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

        //DI para el servicio de fixtures y el logger
        public FixturesController(IFixtureService fixtureService, ILogger<FixturesController> logger)
        {
            _fixtureService = fixtureService;
            _logger = logger;
        }

        // POST: api/fixtures/generate-by-category
        // Body: { "tournamentId": 1, "categoryId": 2 }
        /// <summary>
        /// Genera el fixture de una categoría para un torneo.
        /// </summary>
        /// <remarks>
        /// Crea automáticamente los partidos entre los equipos pertenecientes
        /// a la categoría y al torneo seleccionados.
        ///
        /// El formato del fixture depende de la configuración del torneo:
        ///
        /// - SingleRoundRobin: todos contra todos, solo ida.
        /// - DoubleRoundRobin: todos contra todos, ida y vuelta.
        ///
        /// No se permite generar nuevamente un fixture que ya existe.
        /// Tampoco se puede generar o modificar el fixture de un torneo finalizado.
        ///
        /// Requiere autenticación mediante JWT y rol Admin.
        /// </remarks>
        /// <param name="dto">
        /// Datos del torneo y la categoría necesarios para generar el fixture.
        /// </param>
        /// <returns>
        /// Información del fixture generado.
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("generate-by-category")]
        public async Task<IActionResult> GenerateByCategory(GenerateFixtureByCategoryDto dto)
        {
            //Eliminamos el try-catch para que los errores se manejen globalmente en el middleware de excepciones
            //El logging se mantiene para registrar el inicio y fin del proceso, así como cualquier información relevante

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


        // POST: api/fixtures/assign-dates
        // Body: { "tournamentId": 1, "categoryId": 2, "startDate": "2024-07-01T10:00:00", "endDate": "2024-07-31T18:00:00" }
        /// <summary>
        /// Asigna fechas y horarios a los partidos del fixture.
        /// </summary>
        /// <remarks>
        /// Programa automáticamente los partidos utilizando la fecha inicial,
        /// los horarios configurados, la duración de cada partido y la pausa
        /// definida entre los turnos.
        ///
        /// No se pueden asignar ni modificar fechas cuando el torneo se
        /// encuentra finalizado.
        ///
        /// Requiere autenticación mediante JWT y rol Admin.
        /// </remarks>
        /// <param name="dto">
        /// Configuración utilizada para asignar las fechas y los horarios.
        /// </param>
        /// <returns>
        /// Confirmación de la asignación de fechas.
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("assign-dates")]
        public async Task<IActionResult> AssignDates(AssignMatchDatesDto dto)
        {
            //Eliminamos el try-catch para que los errores se manejen globalmente en el middleware de excepciones
            //El logging se mantiene para registrar el inicio y fin del proceso, así como cualquier información relevante

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


        // DELETE: api/fixtures/tournament/1/category/2
        // Elimina el fixture completo para un torneo y categoría específicos
        /// <summary>
        /// Elimina el fixture de una categoría.
        /// </summary>
        /// <remarks>
        /// Elimina los partidos generados para el torneo y la categoría
        /// seleccionados.
        ///
        /// Esta operación no está permitida cuando el torneo se encuentra
        /// finalizado.
        ///
        /// Requiere autenticación mediante JWT y rol Admin.
        /// </remarks>
        /// <param name="tournamentId">
        /// Identificador del torneo.
        /// </param>
        /// <param name="categoryId">
        /// Identificador de la categoría.
        /// </param>
        /// <returns>
        /// Confirmación de la eliminación del fixture.
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("tournament/{tournamentId}/category/{categoryId}")]
        public async Task<IActionResult> DeleteFixtureByCategory(int tournamentId, int categoryId)
        {
            //Eliminamos el try-catch para que los errores se manejen globalmente en el middleware de excepciones
            //El logging se mantiene para registrar el inicio y fin del proceso, así como cualquier información relevante

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


        // POST: api/fixtures/assign-fields
        // Body: { "tournamentId": 1, "categoryId": 2 }
        /// <summary>
        /// Asigna canchas a los partidos del fixture.
        /// </summary>
        /// <remarks>
        /// Distribuye las canchas disponibles entre los partidos pertenecientes
        /// al torneo y a la categoría seleccionados.
        ///
        /// No se pueden asignar ni modificar canchas cuando el torneo se
        /// encuentra finalizado.
        ///
        /// Requiere autenticación mediante JWT y rol Admin.
        /// </remarks>
        /// <param name="tournamentId">
        /// Identificador del torneo.
        /// </param>
        /// <param name="categoryId">
        /// Identificador de la categoría.
        /// </param>
        /// <returns>
        /// Confirmación de la asignación de canchas.
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("assign-fields")]
        public async Task<IActionResult> AssignFields(int tournamentId, int categoryId)
        {
            //Eliminamos el try-catch para que los errores se manejen globalmente en el middleware de excepciones
            //El logging se mantiene para registrar el inicio y fin del proceso, así como cualquier información relevante

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

        // PENDING: Implementar el endpoint para consultar el fixture por ronda
        //2. Endpoint para consultar el fixture por ronda


    }
}
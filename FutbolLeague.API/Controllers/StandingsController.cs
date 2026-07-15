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
        /// <summary>
        /// Obtiene la tabla de posiciones completa de un torneo.
        /// </summary>
        /// <remarks>
        /// Calcula la tabla utilizando únicamente los partidos que tienen
        /// estado Played y cuyos resultados fueron cargados.
        ///
        /// La tabla incluye:
        ///
        /// - Posición.
        /// - Partidos jugados.
        /// - Partidos ganados.
        /// - Partidos empatados.
        /// - Partidos perdidos.
        /// - Goles a favor.
        /// - Goles en contra.
        /// - Diferencia de gol.
        /// - Puntos.
        ///
        /// El orden se determina por:
        ///
        /// 1. Mayor cantidad de puntos.
        /// 2. Mayor diferencia de gol.
        /// 3. Mayor cantidad de goles a favor.
        /// 4. Nombre del equipo en orden alfabético.
        ///
        /// Este endpoint es público y no requiere autenticación.
        /// </remarks>
        /// <param name="tournamentId">
        /// Identificador del torneo.
        /// </param>
        /// <returns>
        /// Tabla de posiciones completa del torneo.
        /// </returns>
        [HttpGet("tournament/{tournamentId}")]
        public async Task<IActionResult> GetByTournament(int tournamentId)
        {
            var standings = await _standingService.GetStandingsByTournamentAsync(tournamentId);

            if (!standings.Any())
                return NotFound( $"No hay equipos registrados para el torneo " + $"{tournamentId}");

            return Ok(standings);
        }

        // GET: api/Standings/tournament/5/category/3
        // Obtener la tabla de posiciones para un torneo específico y una categoría específica
        /// <summary>
        /// Obtiene la tabla de posiciones de una categoría.
        /// </summary>
        /// <remarks>
        /// Calcula la tabla de posiciones correspondiente al torneo y
        /// a la categoría seleccionados.
        ///
        /// Solo se consideran los partidos con estado Played que tengan
        /// cargados los goles del equipo local y visitante.
        ///
        /// La tabla se ordena por puntos, diferencia de gol, goles a favor
        /// y nombre del equipo.
        ///
        /// Este endpoint es público y no requiere autenticación.
        /// </remarks>
        /// <param name="tournamentId">
        /// Identificador del torneo.
        /// </param>
        /// <param name="categoryId">
        /// Identificador de la categoría.
        /// </param>
        /// <returns>
        /// Tabla de posiciones del torneo y la categoría seleccionados.
        /// </returns>
        [HttpGet("tournament/{tournamentId}/category/{categoryId}")]
        public async Task<IActionResult> GetByTournamentAndCategory(int tournamentId, int categoryId)
        {
            var standings = await _standingService.GetStandingsByTournamentAndCategoryAsync(tournamentId, categoryId);

            if (!standings.Any())
                return NotFound(
                    $"No hay equipos registrados para el torneo " +
                    $"{tournamentId} y la categoría {categoryId}");

            return Ok(standings);
        }
    }
}


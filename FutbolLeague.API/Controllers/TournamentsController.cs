using FutbolLeague.Application.DTOs;
using FutbolLeague.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/*
 * =================================
 * Ayuda de Visual Studio para trabajar con el código
 * =================================
 * 
    Ctrl + M, O para colapsar todo el código y tener una vista general del controlador
    Ctrl + M, L para expandir todo el código y ver los detalles del controlador
    Ctrl + M, P para expandir el bloque de código actual y ver los detalles del método o sección en la que estamos trabajando
    Ctrl + K, C para comentar un bloque de código
    Ctrl + K, U para descomentar un bloque de código
    Embellecer el codigo con Ctrl + K, D 
*/

namespace FutbolLeague.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TournamentsController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;
        private readonly ILogger<TournamentsController> _logger;

        public TournamentsController(ITournamentService tournamentService, ILogger<TournamentsController> logger)
        {
            _tournamentService = tournamentService;
            _logger = logger;
        }


        //GET: api/tournaments
        //Devuelve la lista de torneos disponibles con su formato de fixture
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tournaments = await _tournamentService.GetAllAsync(); // Llamada al servicio para obtener la lista de torneos
            return Ok(tournaments);
        }


        //POST: api/tournaments
        //Crea un nuevo torneo con el nombre y formato de fixture especificados
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateTournamentDto dto)
        {
            _logger.LogInformation("Creando torneo con nombre {TournamentName}", dto.Name);

            var tournament = await _tournamentService.CreateAsync(dto); // Llamada al servicio para crear un nuevo torneo con los datos proporcionados en el DTO

            _logger.LogInformation("Torneo creado correctamente con Id={TournamentId}", tournament.Id);

            return Ok(tournament);
        }


        //GET: api/tournaments/{tournamentId}/summary/{categoryId}
        //Devuelve el resumen del torneo para una categoría específica, incluyendo la tabla de posiciones, los partidos jugados y los próximos partidos
        [HttpGet("{tournamentId}/summary/{categoryId}")]
        public async Task<IActionResult> GetSummary(int tournamentId, int categoryId)
        {
            _logger.LogInformation(
                "Consultando resumen TournamentId={TournamentId}, CategoryId={CategoryId}",
                tournamentId,
                categoryId);

            var summary = await _tournamentService.GetSummaryAsync(tournamentId, categoryId);

            return Ok(summary);
        }

        //Put: api/tournaments/{id}/start
        //Inicia el torneo, cambiando su estado a "En curso" y generando los partidos correspondientes al formato de fixture seleccionado
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/start")]
        public async Task<IActionResult> Start(int id)
        {
            var result = await _tournamentService.StartAsync(id);

            return Ok(result);
        }

        //Put: api/tournaments/{id}/finish
        //Finaliza el torneo, cambiando su estado a "Finalizado" y actualizando los resultados de los partidos restantes como perdidos para los equipos que no hayan jugado todos sus partidos
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/finish")]
        public async Task<IActionResult> Finish(int id)
        {
            var result = await _tournamentService.FinishAsync(id);

            return Ok(result);
        }
    }
}
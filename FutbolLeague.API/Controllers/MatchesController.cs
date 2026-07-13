using FutbolLeague.Application.DTOs;
using FutbolLeague.Application.Exceptions;
using FutbolLeague.Application.Services;
using FutbolLeague.Domain;
using FutbolLeague.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FutbolLeague.API.Controllers
{
    //Ctrol + M, O para colapsar todo el código y tener una vista general del controlador
    //Ctrol + M, P para expandir todo el código y ver los detalles de cada método
    //Ctrol + K, D para organizar el código y mejorar la legibilidad

    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController : ControllerBase
    {
        // Inyectamos el contexto de la base de datos a través del constructor
        private readonly AppDbContext _context; // Inyección de contexto de base de datos
        private readonly IMatchService _matchService; // Inyección del servicio de gestión de partidos
        private readonly ILogger<MatchesController> _logger; // Inyección de logger

        //Constructor del controlador con inyección de dependencias
        public MatchesController(AppDbContext context, IMatchService matchService, ILogger<MatchesController> logger)
        {
            _context = context;
            _matchService = matchService;
            _logger = logger;
        }


        //GET: api/Matches
        /// <summary>
        /// Obtiene todos los partidos de la base de datos, incluyendo información del torneo, categoría, equipos y campo. Los resultados se ordenan por torneo, ronda y fecha del partido.
        /// </summary>
        /// <remarks>
        /// Devuelve los partidos con información del torneo, estado del torneo,
        /// categoría, equipos local y visitante, resultado, estado del partido,
        /// fecha y cancha asignada.
        ///
        /// Este endpoint es público y no requiere autenticación.
        /// </remarks>
        /// <returns>Lista completa de partidos.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var matches = await _context.Matches
                .Include(m => m.Tournament)
                .Include(m => m.HomeTeam)
                    .ThenInclude(t => t.Category)
                .Include(m => m.AwayTeam)
                .Include(m => m.Field)
                .OrderBy(m => m.TournamentId)
                .ThenBy(m => m.Round)
                .ThenBy(m => m.MatchDate)
                .Select(m => new MatchDto
                {
                    Id = m.Id,

                    TournamentId = m.TournamentId,
                    TournamentName = m.Tournament!.Name,
                    TournamentStatus = m.Tournament.Status.ToString(),

                    CategoryId = m.HomeTeam!.CategoryId,
                    CategoryName = m.HomeTeam.Category!.Name,

                    Round = m.Round,

                    HomeTeamId = m.HomeTeamId,
                    HomeTeam = m.HomeTeam.Name,

                    AwayTeamId = m.AwayTeamId,
                    AwayTeam = m.AwayTeam!.Name,

                    HomeScore = m.HomeScore,
                    AwayScore = m.AwayScore,

                    Status = m.Status.ToString(),

                    MatchDate = m.MatchDate,

                    FieldId = m.FieldId,
                    Field = m.Field != null ? m.Field.Name : null
                })
                .ToListAsync();

            return Ok(matches);
        }


        // Post: api/Matches
        /// <summary>
        /// Crear un partido manualmente. Se valida que los equipos no sean iguales y que existan en la base de datos. Solo los usuarios con rol "Admin" pueden crear partidos.
        /// </summary>
        /// /// <remarks>
        /// Registra un nuevo partido entre un equipo local y un equipo visitante.
        ///
        /// Este endpoint permite crear un partido individual. Para generar
        /// automáticamente todos los partidos de una categoría se recomienda
        /// utilizar el endpoint correspondiente de Fixtures.
        ///
        /// Requiere autenticación mediante JWT y rol Admin.
        /// </remarks>
        /// <param name="dto">Datos necesarios para crear el partido.</param>
        /// <returns>El partido creado.</returns>
        /// <exception cref="BusinessException"></exception>
        /// <exception cref="NotFoundException"></exception>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateMatchDto dto)
        {
            if (dto.HomeTeamId == dto.AwayTeamId)
                //return BadRequest("Un equipo no puede jugar contra sí mismo");
                throw new BusinessException("Un equipo no puede jugar contra sí mismo");

            var homeExists = await _context.Teams.AnyAsync(t => t.Id == dto.HomeTeamId);
            var awayExists = await _context.Teams.AnyAsync(t => t.Id == dto.AwayTeamId);

            if (!homeExists || !awayExists)
                //return BadRequest("Uno de los equipos no existe");
                throw new NotFoundException("Uno de los equipos no existe");

            var match = new Match
            {
                HomeTeamId = dto.HomeTeamId,
                AwayTeamId = dto.AwayTeamId
            };

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            return Ok(match);
        }



        //Get api/Matches/tournament/{tournamentId}/round/{round}
        /// <summary>
        /// Obtiene los partidos de una ronda de un torneo.
        /// </summary>
        /// <remarks>
        /// Devuelve todos los partidos correspondientes al torneo y al número
        /// de ronda indicados, sin filtrar por categoría.
        ///
        /// Este endpoint es público y no requiere autenticación.
        /// </remarks>
        /// <param name="tournamentId">Identificador del torneo.</param>
        /// <param name="round">Número de ronda del fixture.</param>
        /// <returns>Lista de partidos de la ronda seleccionada.</returns>
        [HttpGet("tournament/{tournamentId}/round/{round}")]
        public async Task<IActionResult> GetByRound(int tournamentId, int round)
        {
            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.TournamentId == tournamentId && m.Round == round)
                .Select(m => new MatchDto
                {
                    Id = m.Id,
                    Round = m.Round,
                    HomeTeam = m.HomeTeam.Name,
                    AwayTeam = m.AwayTeam.Name,
                    HomeScore = m.HomeScore,
                    AwayScore = m.AwayScore,
                    Status = m.Status.ToString(), //Devolvemos el estado como string para mayor claridad
                    MatchDate = m.MatchDate, //Incluimos la fecha del partido en el DTO
                    Field = m.Field != null ? m.Field.Name : null //Incluimos el nombre del campo si existe
                })
                .ToListAsync();

            if (!matches.Any())
                return NotFound("No hay partidos para esa ronda");

            //return Ok(matches);
            return Ok(new
            {
                TournamentId = tournamentId,
                Round = round,
                Matches = matches
            });
        }



        //Get api/Matches/tournament/{tournamentId}/category/{categoryId}/round/{round}
        /// <summary>
        /// Obtiene los partidos de una categoría y ronda determinadas.
        /// </summary>
        /// <remarks>
        /// Devuelve los partidos pertenecientes al torneo, categoría y número
        /// de ronda especificados.
        ///
        /// Este endpoint es público y no requiere autenticación.
        /// </remarks>
        /// <param name="tournamentId">Identificador del torneo.</param>
        /// <param name="categoryId">Identificador de la categoría.</param>
        /// <param name="round">Número de ronda del fixture.</param>
        /// <returns>Lista de partidos que coinciden con los filtros.</returns>
        [HttpGet("tournament/{tournamentId}/category/{categoryId}/round/{round}")]
        public async Task<IActionResult> GetByTournamentCategoryAndRound(int tournamentId, int categoryId, int round)
        {
            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.TournamentId == tournamentId
                        && m.Round == round
                        && m.HomeTeam.CategoryId == categoryId
                        && m.AwayTeam.CategoryId == categoryId)
                .OrderBy(m => m.Id)
                .Select(m => new MatchDto
                {
                    Id = m.Id,
                    Round = m.Round,
                    HomeTeam = m.HomeTeam.Name,
                    AwayTeam = m.AwayTeam.Name,
                    HomeScore = m.HomeScore,
                    AwayScore = m.AwayScore,
                    Status = m.Status.ToString(), //Devolvemos el estado como string para mayor claridad
                    MatchDate = m.MatchDate, //Incluimos la fecha del partido en el DTO
                    Field = m.Field != null ? m.Field.Name : null //Incluimos el nombre del campo si existe
                })
                .ToListAsync();

            if (!matches.Any()) return NotFound("No hay partidos para esa ronda y categoría");

            return Ok(new
            {
                TournamentId = tournamentId,
                CategoryId = categoryId,
                Round = round,
                Matches = matches
            });

        }



        //Get api/Matches/tournament/{tournamentId}/category/{categoryId}
        /// <summary>
        /// Obtiene todos los partidos de una categoría dentro de un torneo.
        /// </summary>
        /// <remarks>
        /// Devuelve el fixture completo correspondiente al torneo y a la
        /// categoría seleccionados.
        ///
        /// Este endpoint es público y no requiere autenticación.
        /// </remarks>
        /// <param name="tournamentId">Identificador del torneo.</param>
        /// <param name="categoryId">Identificador de la categoría.</param>
        /// <returns>Lista completa de partidos de la categoría.</returns>
        [HttpGet("tournament/{tournamentId}/category/{categoryId}")]
        public async Task<IActionResult> GetByTournamentAndCategory(int tournamentId, int categoryId)
        {
            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.TournamentId == tournamentId
                            && m.HomeTeam.CategoryId == categoryId
                            && m.AwayTeam.CategoryId == categoryId)
                .OrderBy(m => m.Round)
                .ThenBy(m => m.Id)
                .Select(m => new MatchDto
                {
                    Id = m.Id,
                    Round = m.Round,
                    HomeTeam = m.HomeTeam.Name,
                    AwayTeam = m.AwayTeam.Name,
                    HomeScore = m.HomeScore,
                    AwayScore = m.AwayScore,
                    Status = m.Status.ToString(), //Devolvemos el estado como string para mayor claridad
                    MatchDate = m.MatchDate, //Incluimos la fecha del partido en el DTO  
                    Field = m.Field != null ? m.Field.Name : null //Incluimos el nombre del campo si existe
                })
                .ToListAsync();

            if (!matches.Any())
                return NotFound("No hay partidos para ese torneo y categoría");

            return Ok(matches);
        }


        /// <summary>
        /// Registra o actualiza el resultado de un partido.
        /// </summary>
        /// <remarks>
        /// Actualiza los goles del equipo local y visitante.
        ///
        /// Al cargar el resultado, el estado del partido cambia automáticamente
        /// a Played.
        ///
        /// Los goles no pueden contener valores negativos.
        /// No se pueden modificar resultados de torneos finalizados.
        ///
        /// Requiere autenticación mediante JWT y rol Admin.
        /// </remarks>
        /// <param name="id">Identificador del partido.</param>
        /// <param name="dto">Goles del equipo local y visitante.</param>
        /// <returns>Información del resultado actualizado.</returns>        
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/result")]
        public async Task<IActionResult> UpdateResult(int id, UpdateMatchResultDto dto)
        {
            _logger.LogInformation("Actualizando resultado para MatchId={MatchId}", id);

            var result = await _matchService.UpdateResultAsync(id, dto);

            return Ok(result);
        }


        //Put api/Matches/{id}/date
        /// <summary>
        /// Actualiza la fecha y hora de un partido.
        /// </summary>
        /// <remarks>
        /// Modifica la fecha y el horario programados para el partido.
        ///
        /// No se puede modificar la fecha de un partido perteneciente a un
        /// torneo finalizado.
        ///
        /// Requiere autenticación mediante JWT y rol Admin.
        /// </remarks>
        /// <param name="id">Identificador del partido.</param>
        /// <param name="dto">Nueva fecha y hora del partido.</param>
        /// <returns>Información de la fecha actualizada.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/date")]
        public async Task<IActionResult> UpdateDate(int id, UpdateMatchDateDto dto)
        {
            var result = await _matchService.UpdateDateAsync(id, dto);
            return Ok(result);
        }



        //Put api/Matches/{id}/status
        /// <summary>
        /// Actualiza el estado de un partido.
        /// </summary>
        /// <remarks>
        /// Permite modificar el estado actual del partido utilizando uno de
        /// los valores definidos por MatchStatus.
        ///
        /// No se puede modificar el estado de un partido perteneciente a un
        /// torneo finalizado.
        ///
        /// Requiere autenticación mediante JWT y rol Admin.
        /// </remarks>
        /// <param name="id">Identificador del partido.</param>
        /// <param name="status">Nuevo estado del partido.</param>
        /// <returns>Información del estado actualizado.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var result = await _matchService.UpdateStatusAsync(id, status);
            return Ok(result);
        }


        //Put api/Matches/{id}/field
        /// <summary>
        /// Asigna o modifica la cancha de un partido.
        /// </summary>
        /// <remarks>
        /// Asigna una cancha al partido indicado o reemplaza la cancha
        /// previamente asignada.
        ///
        /// No se debería permitir modificar la cancha cuando el torneo se
        /// encuentra finalizado.
        ///
        /// Requiere autenticación mediante JWT y rol Admin.
        /// </remarks>
        /// <param name="id">Identificador del partido.</param>
        /// <param name="dto">Datos de la cancha que se asignará al partido.</param>
        /// <returns>Confirmación de la asignación de la cancha.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/field")]
        public async Task<IActionResult> AssignField(int id, int fieldId)
        {
            var result = await _matchService.UpdateFieldAsync(id, fieldId);

            return Ok(result);
        }


        //Get api/Matches/tournament/{tournamentId}/status/{status}
        /// <summary>
        /// Obtiene los partidos de un torneo filtrados por estado.
        /// </summary>
        /// <remarks>
        /// Devuelve los partidos pertenecientes al torneo indicado cuyo estado
        /// coincide con el valor recibido.
        ///
        /// Este endpoint es público y no requiere autenticación.
        /// </remarks>
        /// <param name="tournamentId">Identificador del torneo.</param>
        /// <param name="status">Estado utilizado para filtrar los partidos.</param>
        /// <returns>Lista de partidos que coinciden con el estado.</returns>
        [HttpGet("tournament/{tournamentId}/status/{status}")]
        public async Task<IActionResult> GetByStatus(int tournamentId, string status)
        {
            if (!Enum.TryParse<MatchStatus>(status, true, out var parsedStatus))
                return BadRequest("Estado inválido. Usar: Pending o Played");

            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.TournamentId == tournamentId && m.Status == parsedStatus)
                .OrderBy(m => m.Round)
                .ThenBy(m => m.MatchDate)
                .Select(m => new MatchDto
                {
                    Id = m.Id,
                    Round = m.Round,
                    HomeTeam = m.HomeTeam.Name,
                    AwayTeam = m.AwayTeam.Name,
                    HomeScore = m.HomeScore,
                    AwayScore = m.AwayScore,
                    Status = m.Status.ToString(),
                    MatchDate = m.MatchDate
                })
                .ToListAsync();

            if (!matches.Any())
                return NotFound("No hay partidos con ese estado");

            return Ok(matches);
        }


        //Get api/Matches/tournament/{tournamentId}/category/{categoryId}/status/{status}
        /// <summary>
        /// Obtiene los partidos de una categoría filtrados por estado.
        /// </summary>
        /// <remarks>
        /// Devuelve los partidos que pertenecen al torneo y a la categoría
        /// indicados, filtrados por el estado recibido.
        ///
        /// Este endpoint es público y no requiere autenticación.
        /// </remarks>
        /// <param name="tournamentId">Identificador del torneo.</param>
        /// <param name="categoryId">Identificador de la categoría.</param>
        /// <param name="status">Estado utilizado para filtrar los partidos.</param>
        /// <returns>Lista de partidos que coinciden con los filtros.</returns>
        [HttpGet("tournament/{tournamentId}/category/{categoryId}/status/{status}")]
        public async Task<IActionResult> GetByTournamentCategoryAndStatus(int tournamentId, int categoryId, string status)
        {
            if (!Enum.TryParse<MatchStatus>(status, true, out var parsedStatus))
                return BadRequest("Estado inválido. Usar: Pending o Played");

            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.Field)
                .Where(m => m.TournamentId == tournamentId
                            && m.Status == parsedStatus
                            && m.HomeTeam.CategoryId == categoryId
                            && m.AwayTeam.CategoryId == categoryId)
                .OrderBy(m => m.Round)
                .ThenBy(m => m.MatchDate)
                .Select(m => new MatchDto
                {
                    Id = m.Id,
                    Round = m.Round,
                    HomeTeam = m.HomeTeam.Name,
                    AwayTeam = m.AwayTeam.Name,
                    HomeScore = m.HomeScore,
                    AwayScore = m.AwayScore,
                    Status = m.Status.ToString(),
                    MatchDate = m.MatchDate,
                    Field = m.Field != null ? m.Field.Name : null
                })
                .ToListAsync();

            if (!matches.Any())
                return NotFound("No hay partidos para ese torneo, categoría y estado");

            return Ok(new
            {
                TournamentId = tournamentId,
                CategoryId = categoryId,
                Status = parsedStatus.ToString(),
                Matches = matches
            });
        }

    }
}
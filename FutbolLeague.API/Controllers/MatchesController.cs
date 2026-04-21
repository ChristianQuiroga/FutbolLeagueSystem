using FutbolLeague.Application.DTOs;
using FutbolLeague.Domain;
using FutbolLeague.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FutbolLeague.API.Controllers
{
    //Ctrol + M, O para colapsar todo el código y tener una vista general del controlador

    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController : ControllerBase
    {
        // Inyectamos el contexto de la base de datos a través del constructor
        private readonly AppDbContext _context;
        public MatchesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Matches/{id}
        //GET: api/Matches
        //Mejora del método GetAll para incluir el nombre de los equipos y ordenar por ronda
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
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

            return Ok(matches);
        }


        // Post: api/Matches
        //Mejora del método Create para validar que los equipos no sean iguales y que existan en la base de datos
        [HttpPost]
        public async Task<IActionResult> Create(CreateMatchDto dto)
        {
            if (dto.HomeTeamId == dto.AwayTeamId)
                return BadRequest("Un equipo no puede jugar contra sí mismo");

            var homeExists = await _context.Teams.AnyAsync(t => t.Id == dto.HomeTeamId);
            var awayExists = await _context.Teams.AnyAsync(t => t.Id == dto.AwayTeamId);

            if (!homeExists || !awayExists)
                return BadRequest("Uno de los equipos no existe");

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
        //Mejora del método GetByRound para incluir el nombre de los equipos y devolver un objeto con la información del torneo, ronda y partidos
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
        //Mejora del método GetByTournamentCategoryAndRound para incluir el nombre de los equipos y devolver un objeto con la información del torneo, categoría, ronda y partidos
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
        //Mejora del método GetByTournamentAndCategory para incluir el nombre de los equipos y devolver un objeto con la información del torneo, categoría y partidos
        //Mejora utíl, consultar todas las rondas de una categoría.
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




        //Put api/Matches/{id}/result
        //Mejora del método updateResult para validar que los goles no sean negativos y devolver un mensaje más detallado al actualizar el resultado
        [HttpPut("{id}/result")]
        public async Task<IActionResult> UpdateResult(int id, UpdateMatchResultDto dto)
        {
            //Mejora del método updateResult
            if (dto.HomeScore < 0 || dto.AwayScore < 0)
                return BadRequest("Los goles no pueden ser negativos");
            //

            var match = await _context.Matches.FindAsync(id);

            if (match == null)
                return NotFound("Partido no encontrado");

            match.HomeScore = dto.HomeScore;
            match.AwayScore = dto.AwayScore;
            match.Status = MatchStatus.Played; //Enun jugado.

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Resultado actualizado",
                MatchId = match.Id,
                HomeScore = match.HomeScore,
                AwayScore = match.AwayScore,
                Status = match.Status.ToString() //Devolvemos el estado como string para mayor claridad
            });
        }


        //Put api/Matches/{id}/status
        //Put endpoint para actualizar la fecha de un partido
        [HttpPut("{id}/date")]
        public async Task<IActionResult> UpdateDate(int id, UpdateMatchDateDto dto)
        {
            var match = await _context.Matches.FindAsync(id);
            
            if (match == null)
                return NotFound("Partido no encontrado");
            
            match.MatchDate = dto.UpdateDate;
            
            await _context.SaveChangesAsync();
            
            return Ok(new
            {
                Message = "Fecha del partido actualizada correctamente",
                MatchId = match.Id,
                MatchDate = match.MatchDate
            });
        }



        //Put api/Matches/{id}/field
        //Put endpoint para asignar una cancha a un partido, validando que no haya conflictos de horarios con otros partidos en la misma cancha
        [HttpPut("{id}/field")]
        public async Task<IActionResult> AssignField(int id, AssignFieldDto dto)
        {
            var match = await _context.Matches.FindAsync(id);

            if (match == null)
                return NotFound("Partido no encontrado");

            if (!match.MatchDate.HasValue)
                return BadRequest("El partido no tiene fecha asignada");

            var conflict = await _context.Matches
                .AnyAsync(m =>
                    m.Id != id &&
                    m.FieldId == dto.FieldId &&
                    m.MatchDate == match.MatchDate);

            if (conflict)
                return BadRequest("Ya existe un partido en esa cancha y horario");

            match.FieldId = dto.FieldId;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Cancha asignada correctamente",
                MatchId = match.Id
            });
        }


        //Get api/Matches/tournament/{tournamentId}/status/{status}
        //Mejora del método GetByStatus para validar el estado y devolver un mensaje más claro si no hay partidos con ese estado
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
        //Mejora del método GetByTournamentCategoryAndStatus para validar el estado y devolver un mensaje más claro si no hay partidos con ese estado, torneo y categoría
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
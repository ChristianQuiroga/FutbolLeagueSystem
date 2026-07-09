using FutbolLeague.Application.DTOs;
using FutbolLeague.Application.Services;
using FutbolLeague.Domain;
using FutbolLeague.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//Ctrol + M, O para colapsar todo el código y tener una vista general del controlador
namespace FutbolLeague.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        // Inyectamos el contexto de la base de datos a través del constructor
        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        // GET: api/Teams
        // Este método obtiene todos los equipos, incluyendo el nombre de su categoría, con DTOs
        //Dtos: Data Transfer Objects, son objetos que se utilizan para transferir datos entre capas de una aplicación, especialmente entre la capa de presentación y la capa de negocio o de acceso a datos. Los DTOs suelen ser clases simples que contienen propiedades para representar los datos que se desean transferir, sin incluir lógica de negocio ni métodos complejos. El uso de DTOs ayuda a desacoplar las diferentes capas de la aplicación, mejorar la seguridad al exponer solo los datos necesarios y facilitar la serialización y deserialización de datos en formatos como JSON o XML.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teams = await _teamService.GetAllAsync();
            return Ok(teams);
        }


        // POST: api/Teams
        // Este método crea un nuevo equipo, validando que el nombre no esté vacío, que la categoría exista, que el nombre no se repita en la misma categoría y que el torneo exista
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateTeamDto dto)
        {
            //Console.WriteLine($"Name: {dto.Name}");
            //Console.WriteLine($"CategoriaId: {dto.CategoryId}");
            //Console.WriteLine($"TournamentId: {dto.TournamentId}");


            var team = await _teamService.CreateAsync(dto);
            return Ok(team);

        }
    }
}
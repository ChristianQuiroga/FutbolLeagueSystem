using FutbolLeague.Application.DTOs;
using FutbolLeague.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FutbolLeague.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Solo los usuarios con el rol "Admin" pueden acceder a este controlador
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Endpoint para restablecer la contraseña de un usuario
        [HttpPut("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id, ResetPasswordDto dto)
        {
            var result = await _userService.ResetPasswordAsync(id, dto); // Llama al servicio para restablecer la contraseña del usuario

            return Ok(result);
        }
    }
}


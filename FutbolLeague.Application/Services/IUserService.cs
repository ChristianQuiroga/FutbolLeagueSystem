using System;
using System.Collections.Generic;
using System.Text;
using FutbolLeague.Application.DTOs;

namespace FutbolLeague.Application.Services
{
    public interface IUserService
    {
        Task<object> ResetPasswordAsync(int userId, ResetPasswordDto dto); // Ajusta el tipo de retorno según tus necesidades
    }
}

// Task hace referencia a una operación asincrónica que puede devolver un resultado. En este caso, el método ResetPasswordAsync devuelve un objeto que representa el resultado de la operación de restablecimiento de contraseña. Puedes ajustar el tipo de retorno según lo que necesites devolver, como un mensaje de éxito, un objeto con información del usuario actualizado, etc.

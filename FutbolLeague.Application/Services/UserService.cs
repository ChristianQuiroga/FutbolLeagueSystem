using BCrypt.Net;
using FutbolLeague.Application.DTOs;
using FutbolLeague.Application.Exceptions;
using FutbolLeague.Infrastructure.Data;

using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Application.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context; // Inyección de dependencias para acceder a la base de datos

        public UserService(AppDbContext context) // Constructor para inyectar el contexto de la base de datos
        {
            _context = context;
        }

        // Implementación del método para restablecer la contraseña de un usuario
        public async Task<object> ResetPasswordAsync(int userId, ResetPasswordDto dto)
        {
            // Primero, busca el usuario en la base de datos
            var user = await _context.Users.FindAsync(userId);

            // Si el usuario no existe, lanza una excepción
            if (user == null)
            {
                throw new NotFoundException("Usuario no encontrado");
            }
            
            if (string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                throw new ArgumentException("La nueva contraseña no puede estar vacía");
            }

            if (dto.NewPassword.Length < 6)
            {
                throw new ArgumentException("La nueva contraseña debe tener al menos 6 caracteres");
            }

            // Si el usuario existe, actualiza su contraseña
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword); 

            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos

            return new
            {
                Message = "Contraseña actualizada exitosamente",
                userId = userId,
                UserName = user.UserName
            }; // Devuelve un objeto con un mensaje de éxito y algunos detalles del usuario
        }
    }

   
}

using FutbolLeague.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Application.Services
{
    // Que hace el task de esta interfaz? Es un contrato que define los métodos para el registro y autenticación de usuarios.
    // El método RegisterAsync toma un RegisterDto como parámetro y devuelve un AuthResponseDto que contiene la información del usuario registrado y un token de autenticación.
    // El método LoginAsync toma un LoginDto como parámetro y devuelve un AuthResponseDto que contiene la información del usuario autenticado y un token de autenticación.
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto); // Method to register a new user
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto); // Method to authenticate a user and return a token
    }
}

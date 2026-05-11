using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Application.DTOs
{
    public class AuthResponseDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}

/*
 * Qué representa cada uno

RegisterDto es lo que enviamos para crear usuario.

LoginDto es lo que enviamos para iniciar sesión.

AuthResponseDto es lo que devuelve la API cuando el login sale bien.*/
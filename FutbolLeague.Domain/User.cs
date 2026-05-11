using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty; 
        // El nombre de usuario del usuario va a ser unico, por lo que se puede usar para identificar al usuario en el sistema.
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User; // Por defecto, el rol del usuario es User, pero se puede cambiar a Admin si es necesario.
    }
}

/*¿Qué es hashing?

Convierte password → texto irreconocible.

Y no puede revertirse 
Usuario escribe:

123456

El backend:

genera hash
guarda hash

En login:

genera hash otra vez
compara hashes
*/
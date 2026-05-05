using System;
using System.Collections.Generic;
using System.Text;

namespace FutbolLeague.Application.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) // Llama al constructor de la clase base (Exception) con el mensaje proporcionado
        { 
        }

    }
}

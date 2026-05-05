using FutbolLeague.Application.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

//Ctrol + M + O para organizar los using
//ctrol + M + L para colapsar todo el código
//Ctrol + M + P para expandir todo el código
//Ctrol + K + D para organizar el código

namespace FutbolLeague.API.Middlewares
{


    // Este middleware se encarga de capturar cualquier excepción no controlada que ocurra durante el procesamiento de las solicitudes HTTP.
    // Middleware para manejar excepciones globalmente
    //Request → Middleware → Controller → Middleware → Response
    public class ExceptionHandlingMiddleware
    {
        // El siguiente middleware en la cadena de procesamiento
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        // Inyección de dependencias para el siguiente middleware y el logger
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo una excepción no controlada");

                context.Response.ContentType = "application/json";

                int statusCode = StatusCodes.Status500InternalServerError; // Código de estado por defecto para errores internos
                string message = "Ocurrió un error interno en el servidor"; // Mensaje por defecto

                // Verificar el tipo de excepción para determinar el código de estado y el mensaje específico
                switch (ex)
                {
                    case BusinessException:
                        statusCode = StatusCodes.Status400BadRequest; // Código de estado para errores de negocio
                        message = ex.Message; // Usar el mensaje específico de la excepción de negocio
                        break;

                    case NotFoundException:
                        statusCode = StatusCodes.Status404NotFound; // Código de estado para recursos no encontrados
                        message = ex.Message;
                        break;

                    case ConflictException:
                        statusCode = StatusCodes.Status409Conflict; // Código de estado para conflictos
                        message = ex.Message;
                        break;
                        // Cómo pensarlo fácil
                        // NotFound: "No encuentro lo que necesito".
                        // Business: "Lo encontré, pero tu pedido no cumple reglas".
                        // Conflict: "Lo encontré, pero choca con un estado actual del sistema".
                }

                context.Response.StatusCode = statusCode;

                var response = new
                {
                    Message = message
                };

                var json = JsonSerializer.Serialize(response); // Convertir el objeto de respuesta a JSON
                await context.Response.WriteAsync(json); // Escribir la respuesta JSON al cliente
            }
        }
        //catch (Exception ex)
        //{
        //    _logger.LogError(ex, "Se produjo una excepción no controlada");

        //    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        //    context.Response.ContentType = "application/json";

        //    var response = new
        //    {
        //        Message = "Ocurrió un error interno en el servidor"
        //    };

        //    var json = JsonSerializer.Serialize(response);
        //    await context.Response.WriteAsync(json);
        //}
    }
}
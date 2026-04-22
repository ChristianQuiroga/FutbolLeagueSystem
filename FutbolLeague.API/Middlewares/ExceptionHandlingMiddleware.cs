using System.Text.Json;

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

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    Message = "Ocurrió un error interno en el servidor"
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
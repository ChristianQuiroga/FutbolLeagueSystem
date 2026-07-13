using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FutbolLeague.API.Swagger
{
    public class AuthorizeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Comprueba si el endpoint o el controller tiene [Authorize].
            var hasAuthorize =
                context.MethodInfo
                    .GetCustomAttributes(true)
                    .OfType<AuthorizeAttribute>()
                    .Any()
                ||
                context.MethodInfo
                    .DeclaringType?
                    .GetCustomAttributes(true)
                    .OfType<AuthorizeAttribute>()
                    .Any() == true;

            // Comprueba si el endpoint permite acceso público.
            var hasAllowAnonymous =
                context.MethodInfo
                    .GetCustomAttributes(true)
                    .OfType<AllowAnonymousAttribute>()
                    .Any();

            // Si el endpoint es público, no agrega candado ni respuestas de seguridad.
            if (!hasAuthorize || hasAllowAnonymous)
                return;

            // Agrega el candado JWT al endpoint protegido.
            operation.Security ??=
                new List<OpenApiSecurityRequirement>();

            operation.Security.Add(
                new OpenApiSecurityRequirement
                {
            {
                new OpenApiSecuritySchemeReference(
                    "Bearer",
                    context.Document),
                new List<string>()
            }
                });

            // Documenta automáticamente la respuesta 401.
            operation.Responses.TryAdd("401", new OpenApiResponse
            {
                Description = "No autorizado. Se requiere un token JWT válido."
            });

            // Documenta automáticamente la respuesta 403.
            operation.Responses.TryAdd(
                "403",
                new OpenApiResponse
                {
                    Description = "Acceso denegado. El usuario no tiene los permisos requeridos."
                });
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FutbolLeague.API.Swagger
{
    public class AuthorizeOperationFilter : IOperationFilter
    {
        public void Apply(
            OpenApiOperation operation,
            OperationFilterContext context)
        {
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

            var hasAllowAnonymous =
                context.MethodInfo
                    .GetCustomAttributes(true)
                    .OfType<AllowAnonymousAttribute>()
                    .Any();

            if (!hasAuthorize || hasAllowAnonymous)
                return;

            operation.Security ??= new List<OpenApiSecurityRequirement>();

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
        }
    }
}
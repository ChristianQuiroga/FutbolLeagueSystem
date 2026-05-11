// See https://aka.ms/new-console-template for more information on how to run this code.
// Conectar tu API con la base de datos usando Entity Framework Core
using FutbolLeague.Infrastructure.Data; // Agregar el using para el contexto de la base de datos
using Microsoft.EntityFrameworkCore;
using FutbolLeague.Application.Services; // Agregar el using para los servicios de la aplicación
using FutbolLeague.API.Middlewares; // Agregar el using para el middleware de manejo de excepciones

using Microsoft.AspNetCore.Authentication.JwtBearer; // Agregar el using para la autenticación JWT
using Microsoft.IdentityModel.Tokens; // Agregar el using para la validación de tokens
using System.Text; // Agregar el using para la codificación de texto (para la clave secreta)
using Microsoft.OpenApi; // Agregar el using para OpenAPI/Swagger


var builder = WebApplication.CreateBuilder(args); // Crear el constructor de la aplicación web

/*Configuración de la autenticación JWT
 */
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey!)
        )
    };
});

builder.Services.AddAuthorization();
// Fin de la configuración de la autenticación JWT


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

//builder.Services.AddOpenApi();
// Agregar los servicios necesarios para la aplicación, incluyendo controladores y servicios personalizados
builder.Services.AddControllers(); // Agregar los controladores a la inyección de dependencias
builder.Services.AddScoped<IStandingService, StandingService>(); // Agregar el servicio de StandingService a la inyección de dependencias
builder.Services.AddScoped<IFixtureService, FixtureService>(); // Agregar el servicio de FixtureService a la inyección de dependencias
builder.Services.AddScoped<IMatchService, MatchService>(); // Agregar el servicio de MatchService a la inyección de dependencias
builder.Services.AddScoped<ITournamentService, TournamentService>(); // Agregar el servicio de TournamentService a la inyección de dependencias

builder.Services.AddScoped<IAuthService, AuthService>(); // Agregar el servicio de AuthService a la inyección de dependencias




//Instalamos los paquetes necesarios para Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "FutbolLeague API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Ingrese solo el token JWT, sin la palabra Bearer."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document),
            new List<string>()
        }
    });
});




// Configurar la conexión a la base de datos!
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseNpgsql("Host=localhost;Database=FutbolLeagueDB;Username=postgres;Password=1234"));  //Cambiar la cadena de conexión según tu configuración
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

var app = builder.Build();


//El middleware tiene que estar antes de que se ejecuten controllers, así atrapa lo que pase después.
app.UseMiddleware<ExceptionHandlingMiddleware>(); // Agregar el middleware de manejo de excepciones a la cadena de procesamiento

app.UseSwagger(); // Habilitar Swagger para generar la documentación de la API
app.UseSwaggerUI(); // Habilitar la interfaz de usuario de Swagger para explorar la documentación de la API


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) // Solo habilitar Swagger en el entorno de desarrollo para evitar exponer la documentación en producción
{
    app.MapOpenApi();
}


app.UseHttpsRedirection(); // Redirigir las solicitudes HTTP a HTTPS para mayor seguridad

app.UseAuthentication(); // Habilitar la autenticación para proteger las rutas de la API
app.UseAuthorization(); // Habilitar la autorización para controlar el acceso a las rutas de la API

app.MapControllers(); // Mapear los controladores a las rutas de la API

app.Run(); // Ejecutar la aplicación




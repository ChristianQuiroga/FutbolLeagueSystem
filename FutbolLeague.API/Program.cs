// See https://aka.ms/new-console-template for more information on how to run this code.
// Conectar tu API con la base de datos usando Entity Framework Core
using FutbolLeague.Infrastructure.Data; // Agregar el using para el contexto de la base de datos
using Microsoft.EntityFrameworkCore;
using FutbolLeague.Application.Services; // Agregar el using para los servicios de la aplicación
using FutbolLeague.API.Middlewares; // Agregar el using para el middleware de manejo de excepciones



var builder = WebApplication.CreateBuilder(args); // Crear el constructor de la aplicación web

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

//builder.Services.AddOpenApi();
builder.Services.AddControllers(); // Agregar los controladores a la inyección de dependencias
builder.Services.AddScoped<IStandingService, StandingService>(); // Agregar el servicio de StandingService a la inyección de dependencias
builder.Services.AddScoped<IFixtureService, FixtureService>(); // Agregar el servicio de FixtureService a la inyección de dependencias



//Instalamos los paquetes necesarios para Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



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

app.MapControllers(); // Mapear los controladores a las rutas de la API

app.Run(); // Ejecutar la aplicación




//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast =  Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast");

//app.Run();

//record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
//{
//    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
//}

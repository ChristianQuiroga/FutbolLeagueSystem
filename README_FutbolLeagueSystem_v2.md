# ⚽ FutbolLeagueSystem

## Sistema de Gestión de Liga Deportiva

Backend desarrollado en .NET para gestionar una liga/campeonato de fútbol con categorías, equipos, fixture, resultados, tabla de posiciones, horarios, canchas y API preparada para una futura app móvil o frontend web.

---

## 🚀 Estado actual del proyecto

El proyecto ya cuenta con una base backend funcional y una arquitectura más ordenada usando:

- Controllers
- Services
- Interfaces
- DTOs
- Middleware global de errores
- Excepciones personalizadas
- Logging
- Entity Framework Core
- PostgreSQL
- Docker para ejecutar la API

---

## 🧱 Arquitectura del Proyecto

```text
FutbolLeagueSystem
│
├── FutbolLeague.API
│   ├── Controllers
│   ├── Middlewares
│   ├── Program.cs
│   └── appsettings.json
│
├── FutbolLeague.Application
│   ├── DTOs
│   ├── Services
│   └── Exceptions
│
├── FutbolLeague.Domain
│   ├── Entities
│   └── Enums
│
└── FutbolLeague.Infrastructure
    ├── Data
    └── Migrations
```

---

## 🔄 Flujo actual de una request

```text
Request
→ Middleware global
→ Controller
→ Service
→ AppDbContext
→ PostgreSQL
→ Response
```

Ejemplo:

```text
FixturesController
→ IFixtureService
→ FixtureService
→ AppDbContext
→ PostgreSQL
```

---

## ⚙️ Tecnologías utilizadas

- ASP.NET Core .NET 10
- Entity Framework Core
- PostgreSQL
- pgAdmin
- Docker
- Swagger / OpenAPI
- Git / GitHub

---

## 🧠 Conceptos aplicados

### Controllers

Los Controllers reciben las peticiones HTTP y devuelven respuestas.

Ejemplo:

- `FixturesController`
- `MatchesController`
- `StandingsController`
- `TournamentsController`
- `TeamsController`
- `CategoriesController`

Actualmente se busca que los Controllers tengan poca lógica y deleguen en Services.

---

### Services

Los Services contienen la lógica principal del negocio.

Servicios actuales:

- `IFixtureService` / `FixtureService`
- `IMatchService` / `MatchService`
- `IStandingService` / `StandingService`
- `ITournamentService` / `TournamentService`

Ejemplo:

```text
Controller = maneja HTTP
Service = ejecuta lógica de negocio
```

---

### Interfaces

Las interfaces funcionan como contratos.

Ejemplo:

```csharp
public interface IFixtureService
{
    Task<object> GenerateByCategoryAsync(GenerateFixtureByCategoryDto dto);
}
```

La implementación real vive en:

```csharp
public class FixtureService : IFixtureService
{
}
```

Esto permite desacoplar el Controller de la clase concreta y mejora la mantenibilidad.

---

### Middleware global de errores

Se creó:

```text
ExceptionHandlingMiddleware
```

Este middleware captura errores globalmente y devuelve respuestas consistentes.

---

### Excepciones personalizadas

Se agregaron excepciones propias para devolver códigos HTTP adecuados:

| Excepción | Código HTTP | Uso |
|---|---:|---|
| `BusinessException` | 400 | Reglas de negocio inválidas |
| `NotFoundException` | 404 | Recurso inexistente |
| `ConflictException` | 409 | Conflictos con datos existentes |
| `Exception` | 500 | Error inesperado |

Ejemplos:

```csharp
throw new NotFoundException("El torneo no existe");
throw new BusinessException("La categoría necesita al menos 2 equipos");
throw new ConflictException("Esa categoría ya tiene fixture generado");
```

---

### Logging

Se agregó logging con `ILogger`.

Sirve para registrar acciones importantes como:

- generar fixture
- asignar fechas
- asignar canchas
- actualizar resultados
- errores inesperados

En Docker se pueden ver logs con:

```bash
docker compose logs -f futbolleague.api
```

---

## 🧩 Entidades principales

### Category

Representa una categoría del torneo.

Ejemplos:

- Honores
- Primera
- Segunda
- Tercera
- Cuarta
- Master

---

### Team

Representa un equipo.

Relaciones:

```text
Team → Category
Team → Tournament
```

---

### Tournament

Representa un torneo/campeonato.

Incluye formato de fixture:

```csharp
public enum FixtureFormat
{
    SingleRoundRobin = 1,
    DoubleRoundRobin = 2
}
```

---

### Match

Representa un partido.

Contiene:

- Torneo
- Equipo local
- Equipo visitante
- Ronda
- Resultado
- Estado
- Fecha/hora
- Cancha

---

### Field

Representa una cancha.

Ejemplos:

- Cancha 1
- Cancha 2
- Cancha 3

---

## 🔗 Relaciones principales

```text
Team → Category
Team → Tournament
Match → Tournament
Match → HomeTeam
Match → AwayTeam
Match → Field
```

---

## ✅ Funcionalidades implementadas

### Categorías

- Crear categoría
- Listar categorías

---

### Equipos

- Crear equipo
- Listar equipos
- Asociar equipo a torneo
- Asociar equipo a categoría
- Validación de duplicados

---

### Torneos

- Crear torneo
- Listar torneos
- Formato configurable:
  - SingleRoundRobin
  - DoubleRoundRobin
- Resumen por torneo y categoría

---

### Fixture

- Generar fixture por categoría
- Soporte Round Robin
- Soporte ida y vuelta
- Rondas automáticas
- Bloquear regeneración si ya hay partidos jugados
- Borrar fixture solo si no hay partidos jugados

---

### Fechas y horarios

Se agregó asignación automática de fechas con lógica real:

- Inicio aproximado por la mañana: 08:30
- Pausa al mediodía
- Reinicio por la tarde: 13:10
- Intervalo entre partidos: 50 minutos

---

### Canchas

- Crear canchas
- Asignar cancha a partido
- Asignar canchas automáticamente
- Validar conflictos básicos de cancha/horario

---

### Partidos

- Listar partidos
- Filtrar por torneo
- Filtrar por categoría
- Filtrar por ronda
- Filtrar por estado
- Cargar resultado
- Actualizar fecha
- Actualizar estado
- Asignar cancha

Estados actuales:

```csharp
public enum MatchStatus
{
    Pending = 1,
    Played = 2
}
```

---

### Tabla de posiciones

Se implementó cálculo automático de standings:

- Posición
- Partidos jugados
- Ganados
- Empatados
- Perdidos
- Goles a favor
- Goles en contra
- Diferencia de gol
- Puntos

Ordenamiento:

1. Puntos
2. Diferencia de gol
3. Goles a favor
4. Nombre del equipo

---

## 📊 Summary del torneo

Se creó endpoint de resumen por torneo y categoría.

Devuelve:

- Próximos partidos
- Últimos resultados
- Tabla de posiciones
- Estadísticas generales

Ejemplo de datos:

```json
{
  "tournamentId": 1,
  "categoryId": 1,
  "nextMatches": [],
  "lastResults": [],
  "standings": [],
  "stats": {
    "totalMatches": 10,
    "playedMatches": 4,
    "pendingMatches": 6,
    "teamsCount": 5
  }
}
```

---

## 🗄️ Base de datos

Base utilizada:

```text
FutbolLeagueDB
```

Motor:

```text
PostgreSQL
```

---

## 🔄 Migraciones

Comandos principales:

```bash
dotnet ef migrations add NombreMigracion --project FutbolLeague.Infrastructure --startup-project FutbolLeague.API
```

```bash
dotnet ef database update --project FutbolLeague.Infrastructure --startup-project FutbolLeague.API
```

---

## 🧪 Swagger

### Ejecución local con Visual Studio

```text
https://localhost:7047/swagger
```

### Ejecución con Docker

```text
http://localhost:8080/swagger
```

---

# 🐳 Docker

## Descripción

El proyecto puede ejecutar la API dentro de Docker.

Actualmente:

- API corre en Docker
- PostgreSQL corre localmente en la máquina

---

## Archivos Docker

En la raíz del proyecto:

```text
Dockerfile
docker-compose.yml
.dockerignore
```

---

## Ejecutar con Docker

Desde la raíz:

```bash
docker compose up --build
```

---

## Detener Docker

```bash
docker compose down
```

---

## Ver logs de la API

```bash
docker compose logs -f futbolleague.api
```

---

## Conexión a PostgreSQL local desde Docker

Cuando la API corre dentro del contenedor, no debe usar `localhost` para acceder a PostgreSQL local.

Debe usar:

```text
host.docker.internal
```

Ejemplo:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=host.docker.internal;Port=5432;Database=FutbolLeagueDB;Username=postgres;Password=TU_PASSWORD"
}
```

Para ejecución local sin Docker se puede usar:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=FutbolLeagueDB;Username=postgres;Password=TU_PASSWORD"
}
```

---

## 📁 Imágenes del README

Si se agregan imágenes al README, crear:

```text
/docs/images/
```

Ejemplos:

- `arquitectura.png`
- `flujo.png`
- `db.png`

---

## 🧹 Limpieza recomendada del proyecto

Archivos que pueden eliminarse si ya no se usan:

- `TestController.cs`
- `TournamentsControllerOLD.cs`

Antes de borrarlos, verificar que no contengan código necesario.

---

## 🚀 Próximos pasos recomendados

### Seguridad

- Agregar autenticación JWT
- Agregar roles:
  - Admin
  - Usuario / consulta

---

### Arquitectura

- Refactor final de controllers restantes
- Evaluar Repository Pattern
- Separar mejor Application de Infrastructure

---

### Validaciones

- Mejorar validaciones de cancha y horario
- Evitar superposición de partidos
- Validar fechas inválidas
- Validar estados permitidos

---

### Funcionalidad

- Agregar árbitros
- Agregar sedes o complejos deportivos
- Agregar frontend web
- Agregar app móvil
- Agregar reportes

---

### Testing

- Unit tests de Services
- Tests de fixture
- Tests de standings
- Tests de reglas de negocio

---

## 📌 Estado general

El proyecto actualmente ya supera un CRUD básico y cuenta con una base sólida de backend real:

- Arquitectura por capas
- Servicios
- Middleware
- Logging
- Excepciones
- Docker
- PostgreSQL
- Lógica de torneo real

---

## Autor

Proyecto desarrollado como práctica profesional backend .NET.

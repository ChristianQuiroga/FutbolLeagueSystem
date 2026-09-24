# ⚽ FutbolLeagueSystem

Backend desarrollado en **.NET / ASP.NET Core** para gestionar una liga de fútbol organizada por categorías.

El sistema permite administrar equipos, torneos, fixtures, fechas, horarios, canchas, resultados y tabla de posiciones. La API está preparada para integrarse posteriormente con un frontend web o una aplicación móvil.

> Proyecto orientado a práctica profesional backend, separación de responsabilidades y arquitectura limpia.

---

## ✨ Funcionalidades principales

### Gestión de la liga

- CRUD de categorías
- CRUD de equipos
- Gestión de torneos
- Fixture automático por categoría
- Resultados de partidos
- Tabla de posiciones
- Asignación de fechas y horarios
- Asignación de canchas
- Estados de partido

### Formatos de torneo

El sistema soporta:

- `SingleRoundRobin`
- `DoubleRoundRobin`

### Tabla de posiciones

El cálculo de standings contempla:

- Puntos
- Diferencia de gol
- Orden automático

### Estados de partido

- `Pending`
- `Played`

---

## 🛠️ Tecnologías utilizadas

- ASP.NET Core (.NET 10)
- C#
- Entity Framework Core
- PostgreSQL
- pgAdmin
- Docker
- Swagger / OpenAPI

---

## 🧱 Arquitectura

El proyecto está organizado en capas para separar responsabilidades:

```text
FutbolLeagueSystem
│
├── FutbolLeague.API
│   └── Controllers / entrada HTTP
│
├── FutbolLeague.Application
│   └── DTOs / lógica de aplicación
│
├── FutbolLeague.Domain
│   └── Entidades del dominio
│
└── FutbolLeague.Infrastructure
    └── Entity Framework Core / acceso a datos
```

Flujo simplificado:

```text
Cliente / App
     │
     ▼
ASP.NET Core API
     │
     ▼
Application
     │
     ▼
Domain
     │
     ▼
Infrastructure
     │
     ▼
PostgreSQL
```

La API también puede ser probada y administrada durante el desarrollo mediante Swagger.

---

## 🧠 Modelo del sistema

### Entidades principales

- `Category`
- `Team`
- `Tournament`
- `Match`
- `Field`

### Relaciones principales

- `Team` → `Category`
- `Team` → `Tournament`
- `Match` → `Team` (`Home` / `Away`)
- `Match` → `Tournament`
- `Match` → `Field`

---

## 📅 Fixtures, fechas y horarios

La generación de fixtures puede realizarse:

- por torneo;
- por categoría.

Las rondas se generan automáticamente.

La asignación de fechas contempla horarios reales de jornada, incluyendo:

- franja de mañana;
- pausa intermedia;
- franja de tarde;
- intervalo configurable entre partidos.

---

## 🗄️ Base de datos

Motor utilizado:

```text
PostgreSQL
```

Base utilizada actualmente:

```text
FutbolLeagueDB
```

Entity Framework Core se utiliza para el acceso a datos y gestión de migraciones.

---

## 🔄 Migraciones

Crear una nueva migración:

```bash
dotnet ef migrations add NombreMigracion --project FutbolLeague.Infrastructure --startup-project FutbolLeague.API
```

Aplicar migraciones:

```bash
dotnet ef database update --project FutbolLeague.Infrastructure --startup-project FutbolLeague.API
```

---

## 🧪 Prueba de la API

Ejecutando el proyecto localmente desde Visual Studio, Swagger está disponible en:

```text
https://localhost:7047/swagger
```

Cuando la API se ejecuta mediante Docker:

```text
http://localhost:8080/swagger
```

---

## 🐳 Docker

El proyecto puede ejecutar la API dentro de Docker.

### Configuración actual

- API: Docker
- PostgreSQL: instalación local

### Requisitos

- Docker Desktop instalado y en ejecución
- PostgreSQL local activo

### Ejecutar

Desde la raíz:

```bash
docker compose up --build
```

Acceder a Swagger:

```text
http://localhost:8080/swagger
```

### Puertos

- Docker: `8080`
- Visual Studio / local: `7047`

### Conexión desde Docker hacia PostgreSQL local

La API utiliza:

```text
host.docker.internal
```

Ejemplo de configuración:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=host.docker.internal;Port=5432;Database=FutbolLeagueDB;Username=postgres;Password=TU_PASSWORD"
}
```

> No versionar contraseñas ni credenciales reales. Utilizar valores locales o mecanismos de configuración seguros.

### Detener contenedores

```bash
docker compose down
```

### Nota

En la configuración actual, Docker no ejecuta PostgreSQL. La base se mantiene local, por lo que PostgreSQL debe estar activo antes de iniciar la API.

---

## 📁 Estructura de imágenes y documentación

Las imágenes utilizadas por el README se encuentran en:

```text
docs/images/
```

Archivos actuales:

- `arquitectura.png`
- `flujo.png`
- `db.png`

### Arquitectura

![Arquitectura](docs/images/arquitectura.png)

### Flujo

![Flujo](docs/images/flujo.png)

### Base de datos

![Base de datos](docs/images/db.png)

---

## 🧩 Creación de la solución

La solución fue organizada en proyectos independientes:

```bash
dotnet new sln -n FutbolLeagueSystem

dotnet new webapi -n FutbolLeague.API
dotnet new classlib -n FutbolLeague.Application
dotnet new classlib -n FutbolLeague.Domain
dotnet new classlib -n FutbolLeague.Infrastructure
```

Referencias entre proyectos:

```bash
dotnet add FutbolLeague.API reference FutbolLeague.Application
dotnet add FutbolLeague.Application reference FutbolLeague.Domain
dotnet add FutbolLeague.Infrastructure reference FutbolLeague.Domain
dotnet add FutbolLeague.API reference FutbolLeague.Infrastructure
```

---

## ✅ Estado actual

Actualmente el backend incluye:

- CRUD de categorías
- CRUD de equipos
- Torneos configurables
- Fixture por categoría
- Rondas automáticas
- Resultados
- Standings
- Fechas automáticas
- Horarios configurables
- Asignación de canchas
- Integración con Docker para la API
- API preparada para futura integración con frontend o app móvil

---

## 🗺️ Próximos pasos

Entre las evoluciones previstas se encuentran:

- Validaciones avanzadas, por ejemplo evitar regenerar fixtures cuando ya existen resultados
- Asignación automática de canchas por horario
- Prevención de solapamientos de partidos
- Gestión de árbitros
- Gestión de sedes
- Autenticación mediante JWT
- Frontend web / aplicación móvil

---

## 👨‍💻 Autor

**Christian Quiroga**

Software Developer | Backend | .NET | C# | Node.js | REST APIs | SQL

GitHub: [ChristianQuiroga](https://github.com/ChristianQuiroga)

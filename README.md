# ⚽ FutbolLeagueSystem

## Sistema de Gestión de Liga Deportiva

Backend desarrollado en .NET para gestionar un campeonato de fútbol con:

- Fixture automático por categoría
- Tabla de posiciones (standings)
- Equipos por categorías
- Fechas y horarios de partidos
- Estado de partidos (Pending / Played)
- API preparada para app móvil
- Docker para ejecución portable

---

## 🧱 Arquitectura del Proyecto

![Arquitectura](docs/images/arquitectura.png)

```text
FutbolLeagueSystem
│
├── FutbolLeague.API            → API (Controllers)
├── FutbolLeague.Application    → DTOs / lógica de aplicación
├── FutbolLeague.Domain         → Entidades del dominio
├── FutbolLeague.Infrastructure → EF Core / Base de datos
```

---

## 🔄 Flujo del sistema

![Flujo](docs/images/flujo.png)

```text
APP → API (.NET) → Base de datos
          ↑
      Admin / Swagger
```

---

## ⚙️ Tecnologías utilizadas

- ASP.NET Core (.NET 10)
- Entity Framework Core
- PostgreSQL
- pgAdmin
- Docker

---

## 🚀 Funcionalidades actuales

✔ CRUD de categorías  
✔ CRUD de equipos  
✔ Torneos con formato configurable:
- SingleRoundRobin
- DoubleRoundRobin  

✔ Generación de fixture:
- Por torneo
- Por categoría  

✔ Rondas automáticas  

✔ Resultados de partidos  

✔ Tabla de posiciones (standings):
- Puntos
- Diferencia de gol
- Orden automático  

✔ Estados de partido:
- Pending
- Played  

✔ Asignación de fechas:
- Horarios reales (mañana + pausa + tarde)
- Intervalo configurable  

✔ Asignación de canchas  

---

## 🧠 Modelo del sistema

### Entidades principales

- Category
- Team
- Tournament
- Match
- Field

### Relaciones clave

- Team → Category
- Team → Tournament
- Match → Team (Home / Away)
- Match → Tournament
- Match → Field

---

## 🗄️ Base de datos

![Base de datos](docs/images/db.png)

Base: `FutbolLeagueDB`

---

## 🔄 Migraciones

```bash
dotnet ef migrations add NombreMigracion --project FutbolLeague.Infrastructure --startup-project FutbolLeague.API

dotnet ef database update --project FutbolLeague.Infrastructure --startup-project FutbolLeague.API
```

---

## 🧪 Test API

Swagger disponible en:

```text
https://localhost:7047/swagger
```

---

# 🐳 Docker

## 📌 Descripción

El proyecto puede ejecutarse en Docker.

👉 Actualmente:
- API corre en Docker
- PostgreSQL corre localmente

---

## 🚀 Requisitos

- Docker Desktop instalado y en ejecución
- PostgreSQL local activo

---

## ▶️ Ejecutar la API con Docker

Desde la raíz del proyecto:

```bash
docker compose up --build
```

---

## 🌐 Acceso a la API

```text
http://localhost:8080/swagger
```

⚠️ Nota:
- Puerto Docker: **8080**
- Puerto local Visual Studio: **7047**

---

## 🔗 Conexión a base de datos

La API usa:

```text
host.docker.internal
```

Ejemplo en `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=host.docker.internal;Port=5432;Database=FutbolLeagueDB;Username=postgres;Password=TU_PASSWORD"
}
```

---

## 🛑 Detener Docker

```bash
docker compose down
```

---

## ⚠️ Notas Docker

- Docker NO ejecuta PostgreSQL en este setup
- Se utiliza PostgreSQL local
- Asegurarse que PostgreSQL esté encendido antes de levantar la API

---

# 🧩 Creación del proyecto

```bash
dotnet new sln -n FutbolLeagueSystem

dotnet new webapi -n FutbolLeague.API
dotnet new classlib -n FutbolLeague.Application
dotnet new classlib -n FutbolLeague.Domain
dotnet new classlib -n FutbolLeague.Infrastructure
```

---

## 🔗 Referencias

```bash
dotnet add FutbolLeague.API reference FutbolLeague.Application
dotnet add FutbolLeague.Application reference FutbolLeague.Domain
dotnet add FutbolLeague.Infrastructure reference FutbolLeague.Domain
dotnet add FutbolLeague.API reference FutbolLeague.Infrastructure
```

---

# 📁 Estructura de imágenes

Crear carpeta:

```text
/docs/images/
```

Agregar:

- arquitectura.png
- flujo.png
- db.png

---

# 🚀 Estado actual del proyecto

✔ Backend funcional completo  
✔ Fixture por categoría  
✔ Standings calculados  
✔ Fechas automáticas  
✔ Docker integrado  
✔ Listo para integración con frontend/app  

---

# 🔥 Próximos pasos

- Validaciones avanzadas (ej: evitar regenerar fixture con resultados)
- Asignación automática de canchas por horario
- Evitar solapamientos de partidos
- Agregar árbitros
- Agregar sedes
- Autenticación (JWT)
- App móvil / frontend

---

# 💡 Notas

- Proyecto orientado a arquitectura limpia
- Backend escalable
- Preparado para producción futura
- Ideal para app mobile

---

# 📌 Autor

Proyecto desarrollado como práctica profesional backend .NET

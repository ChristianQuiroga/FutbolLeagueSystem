# ⚽ FutbolLeagueSystem

## Sistema de Gestión de Liga Deportiva

Backend desarrollado en .NET para gestionar un campeonato de fútbol con:

- Fixture automático
- Tabla de posiciones
- Equipos por categorías
- API preparada para app móvil

---

## 🧱 Arquitectura del Proyecto

![Arquitectura](docs/images/arquitectura.png)

```
FutbolLeagueSystem
│
├── FutbolLeague.API           → API (Controllers)
├── FutbolLeague.Application   → Lógica de negocio
├── FutbolLeague.Domain        → Entidades
├── FutbolLeague.Infrastructure → Base de datos
```

---

## 🔄 Flujo del sistema

![Flujo](docs/images/flujo.png)

```
APP → API (.NET) → Base de datos
          ↑
      Admin Web
```

---

## ⚙️ Tecnologías utilizadas

- ASP.NET Core (.NET)
- Entity Framework Core
- PostgreSQL
- pgAdmin

---

## 🧩 Creación del proyecto

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

## 🌐 Configuración API

```csharp
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=localhost;Database=FutbolLeagueDB;Username=postgres;Password=1234"));

app.MapControllers();
```

---

## 🗄️ Base de datos

![Base de datos](docs/images/db.png)

Base: `FutbolLeagueDB`

---

## 🧠 Entity Framework

Instalar paquetes:

- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Design
- Npgsql.EntityFrameworkCore.PostgreSQL

---

## 🧱 Entidades

### Category

```csharp
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

### Team

```csharp
public class Team
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}
```

---

## 🔄 Migraciones

```bash
dotnet ef migrations add InitialCreate --project FutbolLeague.Infrastructure --startup-project FutbolLeague.API

dotnet ef database update --project FutbolLeague.Infrastructure --startup-project FutbolLeague.API
```

---

## ✅ Resultado

Tablas creadas:

- Categories
- Teams
- __EFMigrationsHistory

---

## 🧪 Test API

```bash
GET /api/test
```

Respuesta:

```
FutbolLeague API funcionando 🚀
```

---

## 🚀 Estado actual

✔ Arquitectura base creada  
✔ API funcionando  
✔ Base de datos conectada  
✔ Migraciones funcionando  

---

## 🔥 Próximos pasos

- CRUD de categorías  
- CRUD de equipos  
- Generador de fixture  
- Tabla de posiciones  
- App móvil  

---

## 📁 Estructura de imágenes (IMPORTANTE)

Crear carpeta en el repo:

```
/docs/images/
```

Y agregar:

- arquitectura.png
- flujo.png
- db.png

---

## 💡 Notas

- Proyecto orientado a costo cero  
- Backend escalable  
- Preparado para app móvil  


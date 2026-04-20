# ✅ FutbolLeagueSystem - Checklist

## 🧱 Base del proyecto

- [x] Crear solución .NET
- [x] Separación en capas (API, Application, Domain, Infrastructure)
- [x] Configuración Entity Framework Core
- [x] Conexión a PostgreSQL

---

## ⚙️ Dominio

- [x] Entidad Category
- [x] Entidad Team
- [x] Entidad Tournament
- [x] Entidad Match
- [x] Enum FixtureFormat
- [x] Enum MatchStatus
- [x] Entidad Field (Cancha)

---

## 🔗 Relaciones

- [x] Team → Category
- [x] Team → Tournament
- [x] Match → Tournament
- [x] Match → HomeTeam / AwayTeam
- [x] Match → Field

---

## 🗄️ Base de datos

- [x] Migraciones funcionando
- [x] Update database correcto
- [x] Backup de base de datos
- [x] Script inicial de datos

---

## 📦 DTOs

- [x] CategoryDto
- [x] TeamDto
- [x] TournamentDto
- [x] MatchDto
- [x] Create DTOs
- [x] UpdateMatchDateDto
- [x] AssignFieldDto

---

## 🌐 API

### Categorías
- [x] Crear
- [x] Listar

### Equipos
- [x] Crear
- [x] Listar
- [x] Validación duplicados

### Torneos
- [x] Crear
- [x] Listar
- [x] FixtureFormat configurable

### Matches
- [x] Crear
- [x] Listar
- [x] Asignar resultado
- [x] Estado (Pending / Played)
- [x] Asignar fecha
- [x] Asignar cancha

---

## 🔄 Fixture

- [x] Generación automática
- [x] Round Robin
- [x] Rondas correctas
- [x] Fixture por categoría

---

## 🕒 Scheduling

- [x] Horario inicial configurable
- [x] Duración de partidos
- [x] Intervalos
- [x] Pausa al mediodía
- [x] Continuación por la tarde

---

## 🏟️ Canchas

- [x] Entidad Field
- [x] Asignación manual
- [x] Asignación automática
- [x] Rotación de canchas
- [x] Evitar solapamientos

---

## 📊 Standings

- [x] Tabla de posiciones
- [x] Puntos
- [x] Diferencia de gol
- [x] Ordenamiento automático

---

## 🐳 Docker

- [x] Dockerfile
- [x] docker-compose.yml
- [x] API corriendo en Docker
- [x] Conexión a PostgreSQL local
- [x] Uso de host.docker.internal

---

## 📄 Documentación

- [x] README actualizado
- [x] Instrucciones de Docker
- [x] CHECKLIST actualizado (este archivo)

---

## 🔥 Pendientes (nivel pro)

### Validaciones
- [ ] No regenerar fixture si hay resultados
- [ ] Validar fechas duplicadas por cancha
- [ ] Validar integridad de torneo

### Sistema
- [ ] Agregar árbitros
- [ ] Agregar sedes
- [ ] Multi-torneo activo

### Seguridad
- [ ] Autenticación JWT
- [ ] Roles (admin / usuario)

### Infraestructura
- [ ] PostgreSQL en Docker
- [ ] Variables de entorno por ambiente
- [ ] Logging estructurado

### Frontend
- [ ] App web o mobile
- [ ] Visualización de fixture
- [ ] Tabla en tiempo real
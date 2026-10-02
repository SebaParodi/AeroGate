# ✈️ AeroGate — Sistema de Gestión de Vuelos y Pasajes

> Proyecto personal · .NET 10 / ASP.NET Core · Clean Architecture

Sistema de gestión para una aerolínea que permite administrar el catálogo de aeropuertos, el ciclo de vida de los vuelos (máquina de estados), la venta y devolución de pasajes con categorías de fidelidad, y la auditoría de todas las operaciones críticas.

---

## 🧰 Tecnologías

| Capa | Tecnología |
|------|-----------|
| Backend API | ASP.NET Core 10 Web API + JWT Bearer |
| ORM | Entity Framework Core 10 (Code First, TPH, Complex Types) |
| Base de datos | SQL Server (SQLEXPRESS / LocalDB) |
| Cliente Web | ASP.NET Core 10 MVC (Cookie Auth + Session) |
| UI | Bootstrap 5 |
| Documentación API | Swagger / OpenAPI |

---

## 🏗️ Arquitectura

El proyecto sigue una arquitectura en capas (Clean Architecture) con **dos aplicaciones independientes**: la API concentra toda la lógica de negocio y el cliente MVC es un consumidor HTTP puro.

```
Project_Sistema_Aeropuerto/
├── LogicaNegocio/        → Dominio (Entidades, Value Objects, Enums, IRepositorios, Excepciones)
├── LogicaAplicacion/     → Casos de Uso (CasosDeUso + ICasosDeUso)
├── LogicaAccesoDatos/    → EF Core, AeroGateContext, Repositorios, Migraciones
├── DTOs/                 → Data Transfer Objects + Mappers
├── AeroGateWebApi/       → API REST (Controllers, JWT, Swagger, Seed)
└── AeroGateWebApp/       → Cliente MVC standalone (fuera de la solución)
```

### Decisiones de diseño destacadas

- **TPH sobre la jerarquía de usuarios**: `Usuario → Cliente | Empleado → Administrador | Operador`, mapeada en una sola tabla con discriminador `TipoUsuario`. El rol del JWT se deriva del tipo concreto.
- **Máquina de estados en el dominio**: `Vuelo.CambiarEstado()` valida las transiciones (`PROGRAMADO → EMBARQUE → EN_VUELO → ATERRIZADO`, con `DEMORADO` y `CANCELADO`). Cancelar un vuelo dispara la **devolución automática del 100 %** de todos los pasajes vendidos.
- **Categorías de fidelidad como enumerado**: `ESTANDAR / FRECUENTE / PREMIUM` con reglas encapsuladas en `Cliente` — descuento en compra (0/10/15 %), reintegro en devolución (70/90/100 %) y recálculo automático según las compras de los últimos 12 meses.
- **Value Objects con Complex Types**: `NombreCompletoVO`, `PasswordVO` (operador implícito + comparación por valor) y `CodigoIATAVO` (3 letras mayúsculas).
- **Seguridad**: el ID del usuario que ejecuta cada operación sale **siempre del claim del JWT**, nunca del body del request — un cliente no puede comprar ni devolver a nombre de otro.
- **Auditoría inmutable**: toda operación crítica registra quién, qué, cuándo y sobre qué entidad. El Administrador ve todas; el Operador, solo las suyas.
- **Dos soluciones independientes**: la API y el cliente MVC corren en procesos separados y se comunican únicamente por HTTP. El MVC no referencia proyectos de negocio ni EF.

---

## ✅ Requerimientos Funcionales

| RF | Descripción | Rol |
|----|-------------|-----|
| RF01 | Registro de cliente con validación de contraseña segura | Público |
| RF02 | Alta de usuarios internos (Administradores y Operadores) | Administrador |
| RF03 | Login con JWT (API) / Cookie + Session (MVC) | Todos |
| RF04 | CRUD de vuelos con baja lógica (no eliminable con pasajes vendidos) | Administrador |
| RF05 | Cambio de estado de vuelo según máquina de estados | Admin / Operador |
| RF06 | Compra de pasaje con control de asientos y descuento por categoría | Cliente |
| RF07 | Devolución con preview del monto y reintegro según categoría | Cliente |
| RF08 | "Mis pasajes" filtrable por mes/año | Cliente |
| RF09 | Pasajeros de un vuelo específico | Admin / Operador |
| RF10 | Ranking de destinos más vendidos | Todos |
| RF11 | Auditoría con filtros por operación y fechas | Admin / Operador |
| RF12 | Gestión de aeropuertos (catálogo IATA con búsqueda) | Admin / Operador |
| RF13 | (Opcional, pendiente) Evaluación de condiciones con IA + fallback | Admin / Operador |

La especificación completa está en [Docs/LETRA.md](Docs/LETRA.md), el modelo de dominio en [Docs/DiagramaClases.md](Docs/DiagramaClases.md), y el contexto del proyecto (flujos de cada requerimiento, reglas de negocio, decisiones de diseño, atributos de calidad y backlog) en [Docs/CONTEXTO_PROYECTO.md](Docs/CONTEXTO_PROYECTO.md).

---

## 🗄️ Base de Datos

**Motor:** SQL Server · **Base:** `AeroGateDb`

Configuración en `AeroGateWebApi/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(local)\\SQLEXPRESS01;Database=AeroGateDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```

> Ajustar el nombre de instancia según el entorno (`SQLEXPRESS`, `(localdb)\MSSQLLocalDB`, etc.).

Las migraciones se aplican **automáticamente** al iniciar la API (`Database.Migrate()`), junto con el seed: un usuario de cada rol, 8 aeropuertos con códigos IATA reales y 8 vuelos con fechas relativas a la fecha de creación de la base. Si la base de desarrollo ya tiene unos meses y no quedan vuelos vendibles, se regenera con `dotnet ef database drop --project LogicaAccesoDatos --startup-project LogicaAccesoDatos --force` y reiniciando la API.

---

## 🚀 Cómo ejecutar

### Paso 0 — Clave JWT (una sola vez)

La clave de firma de los tokens **no se versiona**. En desarrollo se guarda con user-secrets (fuera del repositorio):

```bash
dotnet user-secrets set "Jwt:Key" "<una clave aleatoria de 32 caracteres o más>" --project AeroGateWebApi
```

En producción se define con la variable de entorno `Jwt__Key`. Si falta, la API no arranca e indica cómo configurarla.

### Paso 1 — API

1. Abrir `AeroGate.sln` en Visual Studio.
2. Establecer `AeroGateWebApi` como proyecto de inicio y ejecutar el perfil **http**.
3. Swagger disponible en: `http://localhost:5100`

### Paso 2 — Cliente MVC

1. Abrir `AeroGateWebApp/AeroGateWebApp.csproj` en otra instancia de Visual Studio.
2. Ejecutar el perfil **http** → `http://localhost:5200`

> ⚠️ La API **debe** estar corriendo antes de usar el cliente MVC.

---

## 👤 Credenciales de prueba

| Rol | Email | Contraseña |
|-----|-------|-----------|
| Administrador | `admin@aerogate.com` | `Admin@123` |
| Operador | `operador@aerogate.com` | `Oper@123` |
| Cliente | `cliente@aerogate.com` | `Cliente@123` |

---

## 📡 Endpoints principales (API)

| Método | Ruta | Autorización |
|--------|------|-------------|
| `POST` | `/api/auth/login` | Público |
| `POST` | `/api/usuario/registro` | Público |
| `POST` | `/api/usuario/empleados` | Administrador |
| `GET` | `/api/usuario` | Administrador |
| `GET/POST/PUT` | `/api/aeropuerto` | Admin / Operador (lectura: autenticado) |
| `GET` | `/api/vuelo/disponibles` | Autenticado |
| `POST/PUT/DELETE` | `/api/vuelo` | Administrador |
| `POST` | `/api/vuelo/{id}/estado` | Admin / Operador |
| `POST` | `/api/pasaje` | Cliente |
| `GET` | `/api/pasaje/{id}/monto-devolucion` | Cliente |
| `POST` | `/api/pasaje/{id}/devolucion` | Cliente |
| `GET` | `/api/pasaje/mis-pasajes?mes=&anio=` | Cliente |
| `GET` | `/api/pasaje/pasajeros-por-vuelo/{id}` | Admin / Operador |
| `GET` | `/api/pasaje/ranking-destinos` | Autenticado |
| `GET` | `/api/auditoria` | Admin (todas) / Operador (propias) |

> Documentación interactiva completa en Swagger: `http://localhost:5100`

---

## 🖥️ Vistas por rol (MVC)

- **Administrador**: dashboard con totales · Vuelos (CRUD + estados) · Aeropuertos · Usuarios · Pasajeros x Vuelo · Auditoría completa · Ranking.
- **Operador**: dashboard · Vuelos (cambio de estado) · Aeropuertos · Pasajeros x Vuelo · Auditoría propia · Ranking.
- **Cliente**: dashboard · Comprar pasaje · Mis pasajes (con devolución y preview del reintegro) · Ranking.

---

## 👨‍💻 Autor

**Sebastián Parodi**
Proyecto personal · 2026

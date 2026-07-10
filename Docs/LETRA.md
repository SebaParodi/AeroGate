# ✈️ AeroGate — Sistema de Gestión de Vuelos y Pasajes

> Letra del Obligatorio — Proyecto personal · .NET / ASP.NET Core
> Basado en la metodología del obligatorio de Diseño y Desarrollo de Aplicaciones Web

---

## 1. Escenario

La aerolínea regional **AeroSur**, que opera desde el Aeropuerto Internacional de Carrasco, gestiona actualmente la venta de pasajes y la operativa de sus vuelos mediante planillas y procesos manuales. Esto genera problemas frecuentes: sobreventa de asientos, devoluciones liquidadas incorrectamente, vuelos cuyo estado real nadie conoce con certeza y ninguna trazabilidad sobre **quién** creó, modificó o canceló un vuelo.

La dirección de AeroSur solicita el desarrollo de un sistema web que digitalice la operación completa: registro de clientes con categorías de fidelidad, venta y devolución de pasajes con reglas de negocio claras, gestión del ciclo de vida de los vuelos y auditoría de todas las operaciones críticas.

El sistema debe construirse como **dos aplicaciones independientes (standalone)**: una **API REST** que concentra toda la lógica de negocio y una **aplicación web MVC** que actúa como cliente HTTP puro de esa API.

---

## 2. Roles del sistema

| Rol | Descripción |
|-----|-------------|
| **Administrador** | Personal jerárquico de AeroSur. Gestiona vuelos (CRUD), usuarios internos y accede a la auditoría completa. |
| **Operador** | Personal operativo del aeropuerto. Gestiona los estados de los vuelos, consulta pasajeros por vuelo y accede a la auditoría de sus propias operaciones. |
| **Cliente** | Pasajero registrado. Compra pasajes, solicita devoluciones y consulta su historial. |

Los tres roles se modelan con **herencia** sobre una entidad base `Usuario` (jerarquía de al menos dos niveles, mapeada con TPH):

```
Usuario → Cliente
        → Empleado → Administrador
                   → Operador
```

---

## 3. Dominio

### 3.1 Clientes y categorías

Todo cliente pertenece a una **categoría de fidelidad**, modelada como enumerado (`CategoriaCliente`); las reglas de descuento y reintegro asociadas las encapsula la entidad `Cliente`:

| Categoría | Condición (compras en los últimos 12 meses) | Descuento en compra | Reintegro por devolución |
|-----------|--------------------------------------------|--------------------:|-------------------------:|
| **Estándar** | 0 a 4 | 0 % | 70 % |
| **Frecuente** | 5 a 9 | 10 % | 90 % |
| **Premium** | 10 o más | 15 % | 100 % |

- Al registrarse, todo cliente comienza en categoría **Estándar**.
- Luego de **cada compra**, el sistema recalcula automáticamente la categoría del cliente.
- Una devolución **no** descuenta compras a efectos del cálculo de categoría.

### 3.2 Aeropuertos

El sistema mantiene un **catálogo de aeropuertos** sobre el que se definen las rutas de los vuelos. Cada aeropuerto tiene: **código IATA** único (3 letras mayúsculas, ej. `MVD`, `EZE`, `GRU`), nombre, ciudad y país.

- La gestión del catálogo (alta, modificación y búsqueda con filtros por código, ciudad o país) está a cargo de **Administradores y Operadores**, y toda operación de escritura queda auditada.
- Los aeropuertos **no se eliminan**: son un catálogo estable que los vuelos (incluso los históricos) referencian como origen y destino. El valor del catálogo está en sus validaciones y filtros, no en su baja.

### 3.3 Vuelos y estados

Un **vuelo** tiene: código único (ej. `AS101`), aeropuerto de **origen**, aeropuerto de **destino**, fecha/hora de salida y de llegada estimada, precio base del pasaje, capacidad total de asientos y asientos disponibles.

El ciclo de vida de un vuelo se modela con una **máquina de estados** con transiciones válidas explícitas:

```
PROGRAMADO ──→ EMBARQUE ──→ EN_VUELO ──→ ATERRIZADO
    │  ↑            │
    ↓  │            ↓
  DEMORADO ──→  CANCELADO
    │                ↑
    └────────────────┘
```

| Estado | Detalle |
|--------|---------|
| **PROGRAMADO** | Estado inicial. Único estado en el que el vuelo admite modificaciones de datos. |
| **DEMORADO** | Transitorio. Puede volver a PROGRAMADO (reprogramación), pasar a EMBARQUE o ser CANCELADO. |
| **EMBARQUE** | Se cierra la venta de pasajes y no se admiten más devoluciones. |
| **EN_VUELO** | El vuelo despegó. |
| **ATERRIZADO** | Estado final exitoso. |
| **CANCELADO** | Estado final. Dispara la **devolución automática del 100 %** a todos los pasajes vendidos, sin importar la categoría del cliente. |

Cualquier transición fuera de este diagrama debe rechazarse con un error de negocio claro (ej.: un vuelo `ATERRIZADO` no puede pasar a `EMBARQUE`).

### 3.4 Pasajes (compras)

- Un **pasaje** vincula un cliente con un vuelo: fecha de compra, precio final pagado (precio base − descuento por categoría vigente al momento de la compra) y estado del pasaje (`COMPRADO` / `DEVUELTO`).
- Cada compra **decrementa** los asientos disponibles del vuelo; cada devolución los **repone**. El control de asientos vive exclusivamente en la lógica de negocio de la API.
- No se puede comprar un pasaje si: el vuelo no está en estado `PROGRAMADO` o `DEMORADO`, o no quedan asientos disponibles.

### 3.5 Devoluciones

- Un cliente puede devolver un pasaje **solo antes** de que el vuelo pase a `EMBARQUE`.
- El monto a reintegrar se calcula según la **categoría del cliente al momento de la devolución** (ver tabla 3.1).
- La devolución por cancelación del vuelo (100 %) es automática y la genera el sistema, no el cliente.

### 3.6 Auditoría

Toda operación crítica sobre vuelos y pasajes genera un **registro de auditoría** automático e inmutable con: usuario que la realizó, tipo de operación, entidad afectada, fecha/hora y detalle. Operaciones auditadas como mínimo:

`ALTA_VUELO` · `MODIFICACION_VUELO` · `CAMBIO_ESTADO` · `CANCELACION_VUELO` · `BAJA_VUELO` · `COMPRA_PASAJE` · `DEVOLUCION_PASAJE` · `DEVOLUCION_AUTOMATICA` · `ALTA_AEROPUERTO` · `MODIFICACION_AEROPUERTO`

---

## 4. Requerimientos funcionales

| RF | Descripción | Rol |
|----|-------------|-----|
| RF01 | Registro (alta) de cliente con validación de contraseña segura y categoría inicial Estándar | Público |
| RF02 | Alta de usuarios internos (Administradores y Operadores) | Administrador |
| RF03 | Login con JWT (API) / Cookie + Session (MVC) | Todos |
| RF04 | CRUD de vuelos: alta, modificación, baja lógica y listado con filtros | Administrador |
| RF05 | Cambio de estado de vuelo respetando la máquina de estados | Administrador / Operador |
| RF06 | Compra de pasaje con control de asientos y descuento por categoría | Cliente |
| RF07 | Devolución de pasaje con cálculo de reintegro según categoría | Cliente |
| RF08 | "Mis pasajes": historial de compras filtrable por mes/año | Cliente |
| RF09 | Listado de pasajeros de un vuelo específico | Administrador / Operador |
| RF10 | Ranking de destinos con más pasajes vendidos | Todos (autenticados) |
| RF11 | Auditoría de operaciones: el Administrador ve todas, el Operador solo las suyas | Administrador / Operador |
| RF12 | Gestión de aeropuertos: alta, modificación y búsqueda con filtros | Administrador / Operador |
| RF13 | (Opcional) Evaluación con IA de condiciones del vuelo (clima en destino) con fallback por reglas de negocio | Administrador / Operador |

### Detalle de los RF principales

**RF01 — Registro de cliente.** Formulario público. Datos: nombre completo, documento, email (único), dirección y contraseña. La contraseña debe validarse en el dominio (largo mínimo 8, al menos una mayúscula, un número y un carácter especial). El email funciona como identificador de login.

**RF04 — CRUD de vuelos.** Un vuelo solo puede **modificarse** mientras esté `PROGRAMADO`. Un vuelo **con pasajes vendidos no puede eliminarse**: solo puede cancelarse (RF05). Validaciones de alta: código único, aeropuertos de origen y destino existentes y **distintos entre sí**, fecha de salida futura, llegada posterior a la salida, capacidad > 0, precio base > 0.

**RF05 — Cambio de estado.** El sistema ofrece únicamente las transiciones válidas desde el estado actual. Al pasar a `CANCELADO` se ejecuta la devolución automática del 100 % de todos los pasajes en estado `COMPRADO` y se genera una auditoría `DEVOLUCION_AUTOMATICA` por cada uno.

**RF06 — Compra de pasaje.** El cliente ve los vuelos disponibles (en `PROGRAMADO` o `DEMORADO`, con asientos), el precio final con su descuento aplicado, y confirma. La compra decrementa asientos, registra el precio pagado y recalcula la categoría del cliente.

**RF07 — Devolución.** El cliente selecciona uno de sus pasajes activos. El sistema muestra el monto a reintegrar antes de confirmar. Al confirmar: el pasaje pasa a `DEVUELTO`, se repone el asiento y se audita la operación.

**RF11 — Auditoría.** Listado con filtros por tipo de operación y rango de fechas. Cada registro muestra quién, qué, cuándo y sobre qué entidad. Los registros no pueden modificarse ni eliminarse desde ningún rol.

**RF12 — Gestión de aeropuertos.** Alta, modificación y listado con búsqueda por código IATA, ciudad o país (los aeropuertos no se eliminan — regla 3.2). Validaciones: código IATA único con formato de exactamente 3 letras mayúsculas, nombre, ciudad y país obligatorios. Toda alta o modificación genera auditoría con el usuario que la realizó.

**RF13 — (Opcional) IA con fallback.** Antes de confirmar un cambio de estado a `EMBARQUE`, el sistema consulta un modelo de IA sobre las condiciones meteorológicas del destino y sugiere `FAVORABLE` / `ACEPTABLE` / `DESFAVORABLE`. Si la IA no está disponible, se aplica una evaluación de fallback por reglas (ej. por franja horaria o estación del año). La sugerencia **no bloquea** la operación: es informativa.

---

## 5. Requerimientos técnicos (no funcionales)

1. **Arquitectura en capas (Clean Architecture)** con soluciones separadas:
   - Capa de **Dominio**: entidades, value objects, enumerados, interfaces, excepciones propias. Sin dependencias hacia afuera.
   - Capa de **Aplicación**: casos de uso (un caso de uso = una clase, con su interfaz).
   - Capa de **Acceso a Datos**: EF Core Code First, `DbContext`, repositorios.
   - **DTOs + Mappers**: las entidades del dominio nunca viajan por HTTP.
   - **Web API**: controllers finos que delegan en casos de uso. Swagger habilitado.
   - **Web App MVC standalone**: cliente HTTP puro. **No referencia** proyectos de dominio, aplicación ni EF. Toda la lógica vive en la API.
2. **Persistencia**: SQL Server con EF Core Code First. Migraciones aplicadas automáticamente al iniciar la API (`Database.Migrate()`), con **seed** inicial (usuarios de cada rol, vuelos de ejemplo).
3. **Herencia con TPH**: la jerarquía `Usuario → Cliente | Empleado → Administrador | Operador` se mapea en una sola tabla con discriminador.
4. **Value Objects**: `NombreCompletoVO`, `PasswordVO` (con validación y comparación por valor) y `CodigoIATAVO` (3 letras mayúsculas). Mapeados con `[ComplexType]` / Owned Types. El resto de los campos (email, documento, dirección) son `string` validados en el `Validar()` de su entidad.
5. **Autenticación y autorización**: JWT Bearer en la API (expiración configurable) y Cookie Auth + Session en el MVC, que adjunta el JWT en cada llamada. Autorización por rol con `[Authorize(Roles = ...)]` en cada endpoint.
6. **Inyección de dependencias**: todos los casos de uso, repositorios y servicios se registran en `Program.cs`.
7. **Manejo de errores**: excepciones de dominio propias (`VueloException`, `PasajeException`, etc.) traducidas a códigos HTTP correctos (400/401/403/404/409) con mensajes claros.
8. **UI**: Razor Views con Bootstrap 5. Dashboard diferenciado por rol.

---

## 6. Entregables

- Repositorio GitHub con README completo: tecnologías, arquitectura, instrucciones de ejecución, credenciales seed y tabla de endpoints.
- Solución API + solución MVC ejecutables de forma independiente (la API primero).
- Diagrama UML de dominio.
- Scripts SQL de datos de demostración (si aplica).

---

*Proyecto personal de Sebastián Parodi — inspirado en la estructura del obligatorio StellarMinds (Diseño y Desarrollo de Aplicaciones Web, 2026).*

using AeroGateWebApi.CarpetaJWT;
using LogicaAccesoDatos;
using LogicaAccesoDatos.EF.Repositorios;
using LogicaAplicacion.CasosDeUso.CUAeropuerto;
using LogicaAplicacion.CasosDeUso.CUAuditoria;
using LogicaAplicacion.CasosDeUso.CUPasaje;
using LogicaAplicacion.CasosDeUso.CUUsuario;
using LogicaAplicacion.CasosDeUso.CUVuelo;
using LogicaAplicacion.ICasosDeUso.ICUAeropuerto;
using LogicaAplicacion.ICasosDeUso.ICUAuditoria;
using LogicaAplicacion.ICasosDeUso.ICUPasaje;
using LogicaAplicacion.ICasosDeUso.ICUUsuario;
using LogicaAplicacion.ICasosDeUso.ICUVuelo;
using LogicaNegocio.Entidades;
using LogicaNegocio.IRepositorios;
using LogicaNegocio.ValueObjects.VOUsuario;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ── Base de datos ────────────────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AeroGateContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("LogicaAccesoDatos")));

// ── Controladores + JSON (enums como strings) ────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// ── CORS (permite que el cliente MVC consuma la API desde otro puerto) ───────
builder.Services.AddCors(options =>
{
    options.AddPolicy("MvcClient", policy =>
        policy.WithOrigins(
                "http://localhost:5200", "https://localhost:7200")   // AeroGateWebApp
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// ── JWT Authentication ───────────────────────────────────────────────────────
builder.Services.AddJwtAuth(builder.Configuration);

// ── Swagger / OpenAPI ────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AeroGate Web API",
        Version = "v1",
        Description = "API REST para el sistema de gestión de vuelos y pasajes AeroGate."
    });

    // Soporte para Bearer token en Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description  = "JWT Authorization. Formato: Bearer {token}",
        Name         = "Authorization",
        In           = ParameterLocation.Header,
        Type         = SecuritySchemeType.ApiKey,
        Scheme       = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Incluir los comentarios XML de los controllers
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

// ── DI – Repositorios ────────────────────────────────────────────────────────
builder.Services.AddScoped<IRepositorioUsuario, RepositorioUsuario>();
builder.Services.AddScoped<IRepositorioAeropuerto, RepositorioAeropuerto>();
builder.Services.AddScoped<IRepositorioVuelo, RepositorioVuelo>();
builder.Services.AddScoped<IRepositorioPasaje, RepositorioPasaje>();
builder.Services.AddScoped<IRepositorioAuditoria, RepositorioAuditoria>();

// ── DI – Casos de Uso: Usuario ───────────────────────────────────────────────
builder.Services.AddScoped<ICULogin, CULogin>();
builder.Services.AddScoped<ICURegistrarCliente, CURegistrarCliente>();
builder.Services.AddScoped<ICUAltaEmpleado, CUAltaEmpleado>();
builder.Services.AddScoped<ICUListarUsuarios, CUListarUsuarios>();
builder.Services.AddScoped<ICUListarClientes, CUListarClientes>();

// ── DI – Casos de Uso: Aeropuerto ────────────────────────────────────────────
builder.Services.AddScoped<ICUAltaAeropuerto, CUAltaAeropuerto>();
builder.Services.AddScoped<ICUModificarAeropuerto, CUModificarAeropuerto>();
builder.Services.AddScoped<ICUListarAeropuertos, CUListarAeropuertos>();
builder.Services.AddScoped<ICUGetAeropuertoPorId, CUGetAeropuertoPorId>();

// ── DI – Casos de Uso: Vuelo ─────────────────────────────────────────────────
builder.Services.AddScoped<ICUAltaVuelo, CUAltaVuelo>();
builder.Services.AddScoped<ICUModificarVuelo, CUModificarVuelo>();
builder.Services.AddScoped<ICUEliminarVuelo, CUEliminarVuelo>();
builder.Services.AddScoped<ICUListarVuelos, CUListarVuelos>();
builder.Services.AddScoped<ICUGetVueloPorId, CUGetVueloPorId>();
builder.Services.AddScoped<ICUListarVuelosDisponibles, CUListarVuelosDisponibles>();
builder.Services.AddScoped<ICUCambiarEstadoVuelo, CUCambiarEstadoVuelo>();

// ── DI – Casos de Uso: Pasaje ────────────────────────────────────────────────
builder.Services.AddScoped<ICUComprarPasaje, CUComprarPasaje>();
builder.Services.AddScoped<ICUDevolverPasaje, CUDevolverPasaje>();
builder.Services.AddScoped<ICUMisPasajes, CUMisPasajes>();
builder.Services.AddScoped<ICUListarPasajerosPorVuelo, CUListarPasajerosPorVuelo>();
builder.Services.AddScoped<ICURankingDestinos, CURankingDestinos>();

// ── DI – Casos de Uso: Auditoría ─────────────────────────────────────────────
builder.Services.AddScoped<ICUGenerarAuditoria, CUGenerarAuditoria>();
builder.Services.AddScoped<ICUListarAuditorias, CUListarAuditorias>();

var app = builder.Build();

// ── Migraciones + Seed ───────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AeroGateContext>();
    context.Database.Migrate();
    SeedUsuarios(context);
    SeedAeropuertosYVuelos(context);
}

// ── Swagger UI (solo en desarrollo) ─────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AeroGate API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz "/"
    });
}

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseCors("MvcClient");
app.UseAuthentication();   // JWT middleware — ANTES de Authorization
app.UseAuthorization();
app.MapAuthEndpoints();
app.MapControllers();
app.Run();

// ── Seed helpers ─────────────────────────────────────────────────────────────
static void SeedUsuarios(AeroGateContext context)
{
    if (context.Usuarios.Any())
        return;

    context.Usuarios.AddRange(
        new Administrador(
            new NombreCompletoVO("Sebastián", "Parodi"),
            "admin@aerogate.com", "Admin@123",
            new DateTime(2024, 3, 1)),
        new Operador(
            new NombreCompletoVO("Carlos", "Méndez"),
            "operador@aerogate.com", "Oper@123",
            new DateTime(2025, 8, 15)),
        new Cliente(
            new NombreCompletoVO("Juan", "Pérez"),
            "cliente@aerogate.com", "Cliente@123",
            "45678901", "Av. Italia 2345, Montevideo")
    );
    context.SaveChanges();
}

static void SeedAeropuertosYVuelos(AeroGateContext context)
{
    if (context.Aeropuertos.Any())
        return;

    var mvd = new Aeropuerto("MVD", "Aeropuerto Internacional de Carrasco", "Montevideo", "Uruguay");
    var eze = new Aeropuerto("EZE", "Aeropuerto Internacional de Ezeiza", "Buenos Aires", "Argentina");
    var aep = new Aeropuerto("AEP", "Aeroparque Jorge Newbery", "Buenos Aires", "Argentina");
    var gru = new Aeropuerto("GRU", "Aeropuerto Internacional de Guarulhos", "San Pablo", "Brasil");
    var scl = new Aeropuerto("SCL", "Aeropuerto Internacional Arturo Merino Benítez", "Santiago", "Chile");
    var mia = new Aeropuerto("MIA", "Miami International Airport", "Miami", "Estados Unidos");
    var asu = new Aeropuerto("ASU", "Aeropuerto Internacional Silvio Pettirossi", "Asunción", "Paraguay");
    var pdp = new Aeropuerto("PDP", "Aeropuerto Internacional de Punta del Este", "Punta del Este", "Uruguay");

    context.Aeropuertos.AddRange(mvd, eze, aep, gru, scl, mia, asu, pdp);
    context.SaveChanges();

    // Vuelos con fechas relativas a hoy: el seed siempre queda vendible para la demo
    DateTime hoy = DateTime.Today;
    context.Vuelos.AddRange(
        new Vuelo("AS101", mvd.Id, eze.Id, hoy.AddDays(7).AddHours(8),   hoy.AddDays(7).AddHours(9),    180m, 120),
        new Vuelo("AS102", eze.Id, mvd.Id, hoy.AddDays(7).AddHours(18),  hoy.AddDays(7).AddHours(19),   180m, 120),
        new Vuelo("AS201", mvd.Id, gru.Id, hoy.AddDays(10).AddHours(10), hoy.AddDays(10).AddHours(12.5), 320m, 150),
        new Vuelo("AS202", gru.Id, mvd.Id, hoy.AddDays(12).AddHours(15), hoy.AddDays(12).AddHours(17.5), 320m, 150),
        new Vuelo("AS301", mvd.Id, scl.Id, hoy.AddDays(14).AddHours(7),  hoy.AddDays(14).AddHours(9.5),  380m, 140),
        new Vuelo("AS401", mvd.Id, mia.Id, hoy.AddDays(20).AddHours(23), hoy.AddDays(21).AddHours(8),    950m, 200),
        new Vuelo("AS501", mvd.Id, asu.Id, hoy.AddDays(9).AddHours(11),  hoy.AddDays(9).AddHours(13),    260m, 90),
        new Vuelo("AS601", aep.Id, pdp.Id, hoy.AddDays(15).AddHours(9),  hoy.AddDays(15).AddHours(10),   210m, 80)
    );
    context.SaveChanges();
}

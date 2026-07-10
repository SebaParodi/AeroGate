using DTOs.Mappers;
using LogicaAplicacion.ICasosDeUso.ICUUsuario;
using LogicaNegocio.Entidades;
using LogicaNegocio.Excepciones;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AeroGateWebApi.CarpetaJWT
{
    public static class AuthorizationJWT
    {
        // ── Configuración del middleware JWT ─────────────────────────────────
        public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration config)
        {
            var jwtKey      = config["Jwt:Key"];
            var jwtIssuer   = config["Jwt:Issuer"];
            var jwtAudience = config["Jwt:Audience"];

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer           = true,
                        ValidateAudience         = true,
                        ValidateLifetime         = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer              = jwtIssuer,
                        ValidAudience            = jwtAudience,
                        IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            services.AddAuthorization();

            return services;
        }

        // ── Endpoint de login (Minimal API) — RF03 ───────────────────────────
        public static WebApplication MapAuthEndpoints(this WebApplication app)
        {
            app.MapPost("/api/auth/login", [AllowAnonymous]
                (LoginRequest request, ICULogin cuLogin, IConfiguration config) =>
            {
                try
                {
                    Usuario usuario = cuLogin.Ejecutar(request.Email, request.Pass);
                    return Results.Ok(GenerarToken(usuario, config));
                }
                catch (UsuarioException ex)
                {
                    // Credenciales inválidas → 401 con el motivo
                    return Results.Text(ex.Message, statusCode: StatusCodes.Status401Unauthorized);
                }
                catch (Exception ex)
                {
                    return Results.Text(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
                }
            });

            return app;
        }

        // ── Generación del JWT ────────────────────────────────────────────────
        private static LoginResponse GenerarToken(Usuario usuario, IConfiguration config)
        {
            // El rol se deriva del tipo concreto de la jerarquía TPH
            string rol = MapperUsuario.ObtenerRol(usuario);

            var claims = new[]
            {
                new Claim("Id",               usuario.Id.ToString()),
                new Claim(ClaimTypes.Email,   usuario.Email),
                new Claim(ClaimTypes.Name,    usuario.NombreCompleto.Nombre),
                new Claim(ClaimTypes.Surname, usuario.NombreCompleto.Apellido),
                new Claim(ClaimTypes.Role,    rol)
            };

            var key    = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var creds  = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expira = DateTime.UtcNow.AddHours(int.Parse(config["Jwt:ExpiresInHours"] ?? "2"));

            var token = new JwtSecurityToken(
                issuer:             config["Jwt:Issuer"],
                audience:           config["Jwt:Audience"],
                claims:             claims,
                expires:            expira,
                signingCredentials: creds
            );

            return new LoginResponse(
                Token:    new JwtSecurityTokenHandler().WriteToken(token),
                Expira:   expira,
                Id:       usuario.Id,
                Nombre:   usuario.NombreCompleto.Nombre,
                Apellido: usuario.NombreCompleto.Apellido,
                Email:    usuario.Email,
                Rol:      rol
            );
        }
    }

    public record LoginRequest(string Email, string Pass);

    public record LoginResponse(
        string Token, DateTime Expira, int Id, string Nombre,
        string Apellido, string Email, string Rol);
}

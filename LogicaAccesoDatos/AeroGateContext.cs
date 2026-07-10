using LogicaNegocio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace LogicaAccesoDatos
{
    public class AeroGateContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Aeropuerto> Aeropuertos { get; set; }
        public DbSet<Vuelo> Vuelos { get; set; }
        public DbSet<Pasaje> Pasajes { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }

        public AeroGateContext(DbContextOptions<AeroGateContext> options) : base(options) { }

        public AeroGateContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Usuario: herencia TPH + Value Objects ---
            modelBuilder.Entity<Usuario>(e =>
            {
                // Toda la jerarquía en la tabla "Usuarios" con columna discriminadora
                e.HasDiscriminator<string>("TipoUsuario")
                    .HasValue<Cliente>("CLIENTE")
                    .HasValue<Administrador>("ADMINISTRADOR")
                    .HasValue<Operador>("OPERADOR");

                // NombreCompletoVO → columnas NombreCompleto_Nombre / NombreCompleto_Apellido
                // IsRequired: el VO y sus campos son obligatorios (EF necesita al menos
                // una propiedad requerida para distinguir "VO null" de "campos null")
                e.ComplexProperty(u => u.NombreCompleto, nc =>
                {
                    nc.IsRequired();
                    nc.Property(n => n.Nombre).IsRequired();
                    nc.Property(n => n.Apellido).IsRequired();
                });

                // PasswordVO.Valor → columna "Pass" (sin esto sería "Pass_Valor")
                e.ComplexProperty(u => u.Pass, pass =>
                {
                    pass.IsRequired();
                    pass.Property(p => p.Valor).HasColumnName("Pass").IsRequired();
                });

                // El email es el identificador de login: único a nivel de base
                e.HasIndex(u => u.Email).IsUnique();
            });

            // --- Cliente (derivada TPH) ---
            modelBuilder.Entity<Cliente>(e =>
            {
                e.Property(c => c.Categoria).HasConversion<string>();
            });

            // --- Aeropuerto ---
            modelBuilder.Entity<Aeropuerto>(e =>
            {
                // CodigoIATAVO.Valor → columna "CodigoIATA" (la unicidad la controla el CU vía FindByCodigoIATA)
                e.ComplexProperty(a => a.CodigoIATA, c =>
                {
                    c.IsRequired();
                    c.Property(x => x.Valor).HasColumnName("CodigoIATA").IsRequired();
                });
            });

            // --- Vuelo: las DOS relaciones con Aeropuerto, desambiguadas por FK explícita ---
            modelBuilder.Entity<Vuelo>(e =>
            {
                e.HasIndex(v => v.Codigo).IsUnique();
                e.Property(v => v.PrecioBase).HasPrecision(10, 2);
                e.Property(v => v.Estado).HasConversion<string>();

                e.HasOne(v => v.AeropuertoOrigen)
                    .WithMany()
                    .HasForeignKey(v => v.AeropuertoOrigenId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(v => v.AeropuertoDestino)
                    .WithMany()
                    .HasForeignKey(v => v.AeropuertoDestinoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // --- Pasaje ---
            modelBuilder.Entity<Pasaje>(e =>
            {
                e.Property(p => p.PrecioPagado).HasPrecision(10, 2);
                e.Property(p => p.DescuentoAplicado).HasPrecision(5, 4);
                e.Property(p => p.MontoReintegrado).HasPrecision(10, 2);
                e.Property(p => p.Estado).HasConversion<string>();

                e.HasOne(p => p.Cliente)
                    .WithMany(c => c.Pasajes)
                    .HasForeignKey(p => p.ClienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(p => p.Vuelo)
                    .WithMany()
                    .HasForeignKey(p => p.VueloId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // --- Auditoria: inmutable, referencia a Usuario (cualquier tipo de la jerarquía) ---
            modelBuilder.Entity<Auditoria>(e =>
            {
                e.Property(a => a.Operacion).HasConversion<string>();

                e.HasOne(a => a.Usuario)
                    .WithMany()
                    .HasForeignKey(a => a.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Fallback para las herramientas de diseño (migraciones): en runtime la API
            // configura la conexión por DI desde appsettings.json y este bloque no corre.
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(local)\\SQLEXPRESS01;Database=AeroGateDb;Trusted_Connection=True;TrustServerCertificate=True");
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}

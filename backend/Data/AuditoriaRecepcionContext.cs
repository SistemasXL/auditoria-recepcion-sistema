using Microsoft.EntityFrameworkCore;
using AuditoriaRecepcion.Models.Entities;

namespace AuditoriaRecepcion.Data
{
    public class AuditoriaRecepcionContext : DbContext
    {
        public AuditoriaRecepcionContext(DbContextOptions<AuditoriaRecepcionContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<AuditoriaRecepcion.Models.Entities.AuditoriaRecepcion> AuditoriasRecepcion { get; set; }
        public DbSet<DetalleAuditoria> DetallesAuditoria { get; set; }
        public DbSet<Incidencia> Incidencias { get; set; }
        public DbSet<Evidencia> Evidencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones de relaciones
            modelBuilder.Entity<AuditoriaRecepcion.Models.Entities.AuditoriaRecepcion>()
                .HasOne(a => a.UsuarioCreador)
                .WithMany(u => u.AuditoriasCreadas)
                .HasForeignKey(a => a.UsuarioCreadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AuditoriaRecepcion.Models.Entities.AuditoriaRecepcion>()
                .HasOne(a => a.UsuarioAprobador)
                .WithMany(u => u.AuditoriasAprobadas)
                .HasForeignKey(a => a.UsuarioAprobadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Incidencia>()
                .HasOne(i => i.UsuarioReporta)
                .WithMany(u => u.IncidenciasCreadas)
                .HasForeignKey(i => i.UsuarioReportaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Incidencia>()
                .HasOne(i => i.UsuarioAsignado)
                .WithMany(u => u.IncidenciasAsignadas)
                .HasForeignKey(i => i.UsuarioAsignadoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuraciones de índices únicos
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Proveedor>()
                .HasIndex(p => p.Cuit)
                .IsUnique();

            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.Codigo)
                .IsUnique();

            modelBuilder.Entity<AuditoriaRecepcion.Models.Entities.AuditoriaRecepcion>()
                .HasIndex(a => a.NumeroAuditoria)
                .IsUnique();

            modelBuilder.Entity<Incidencia>()
                .HasIndex(i => i.NumeroIncidencia)
                .IsUnique();
        }
    }
}
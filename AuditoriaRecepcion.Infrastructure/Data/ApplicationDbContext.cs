using Microsoft.EntityFrameworkCore;
using AuditoriaRecepcion.Core.Entities;

namespace AuditoriaRecepcion.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Proveedor> Proveedores => Set<Proveedor>();
        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Auditoria> Auditorias => Set<Auditoria>();
        public DbSet<ProductoAuditoria> ProductosAuditoria => Set<ProductoAuditoria>();
        public DbSet<Incidencia> Incidencias => Set<Incidencia>();
        public DbSet<Evidencia> Evidencias => Set<Evidencia>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.Property(e => e.NombreCompleto).HasMaxLength(200).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
            });

            // Configurar Proveedor
            modelBuilder.Entity<Proveedor>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Codigo).IsUnique();
                entity.Property(e => e.Codigo).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Nombre).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Direccion).HasMaxLength(500);
                entity.Property(e => e.Telefono).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Contacto).HasMaxLength(200);
            });

            // Configurar Producto
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Sku).IsUnique();
                entity.HasIndex(e => e.CodigoBarras);
                entity.Property(e => e.Sku).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Nombre).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Descripcion).HasMaxLength(1000);
                entity.Property(e => e.CodigoBarras).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Categoria).HasMaxLength(100);
                entity.Property(e => e.UnidadMedida).HasMaxLength(20).IsRequired();
                entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18,2)");
            });

            // Configurar Auditoria
            modelBuilder.Entity<Auditoria>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.NumeroAuditoria).IsUnique();
                entity.Property(e => e.NumeroAuditoria).HasMaxLength(50).IsRequired();
                entity.Property(e => e.OrdenCompra).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Observaciones).HasMaxLength(2000);

                entity.HasOne(e => e.Proveedor)
                    .WithMany(p => p.Auditorias)
                    .HasForeignKey(e => e.ProveedorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.UsuarioCreador)
                    .WithMany(u => u.Auditorias)
                    .HasForeignKey(e => e.UsuarioCreadorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configurar ProductoAuditoria
            modelBuilder.Entity<ProductoAuditoria>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Observaciones).HasMaxLength(1000);

                entity.HasOne(e => e.Auditoria)
                    .WithMany(a => a.Productos)
                    .HasForeignKey(e => e.AuditoriaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Producto)
                    .WithMany(p => p.ProductosAuditoria)
                    .HasForeignKey(e => e.ProductoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configurar Incidencia
            modelBuilder.Entity<Incidencia>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.NumeroIncidencia).IsUnique();
                entity.Property(e => e.NumeroIncidencia).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Descripcion).HasMaxLength(2000).IsRequired();
                entity.Property(e => e.AccionCorrectiva).HasMaxLength(2000);
                entity.Property(e => e.Resolucion).HasMaxLength(2000);

                entity.HasOne(e => e.Auditoria)
                    .WithMany(a => a.Incidencias)
                    .HasForeignKey(e => e.AuditoriaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ProductoAuditoria)
                    .WithMany()
                    .HasForeignKey(e => e.ProductoAuditoriaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configurar Evidencia
            modelBuilder.Entity<Evidencia>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NombreArchivo).HasMaxLength(255).IsRequired();
                entity.Property(e => e.RutaArchivo).HasMaxLength(500).IsRequired();
                entity.Property(e => e.TipoArchivo).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Descripcion).HasMaxLength(1000);

                entity.HasOne(e => e.Auditoria)
                    .WithMany(a => a.Evidencias)
                    .HasForeignKey(e => e.AuditoriaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ProductoAuditoria)
                    .WithMany()
                    .HasForeignKey(e => e.ProductoAuditoriaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Incidencia)
                    .WithMany()
                    .HasForeignKey(e => e.IncidenciaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
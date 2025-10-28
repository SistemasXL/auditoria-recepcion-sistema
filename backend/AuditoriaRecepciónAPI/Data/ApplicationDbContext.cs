using Microsoft.EntityFrameworkCore;
using AuditoriaRecepcionAPI.Models.Entities;

namespace AuditoriaRecepcionAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<OrdenCompra> OrdenesCompra { get; set; }
        public DbSet<OrdenCompraDetalle> OrdenesCompraDetalle { get; set; }
        public DbSet<AuditoriaRecepcion> AuditoriasRecepcion { get; set; }
        public DbSet<AuditoriaDetalle> AuditoriaDetalle { get; set; }
        public DbSet<Evidencia> Evidencias { get; set; }
        public DbSet<Incidencia> Incidencias { get; set; }
        public DbSet<AuditoriaLog> AuditoriaLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Rol
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(e => e.RolID);
                entity.Property(e => e.NombreRol).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.NombreRol).IsUnique();
            });

            // Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.UsuarioID);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasOne(e => e.Rol).WithMany().HasForeignKey(e => e.RolID).OnDelete(DeleteBehavior.Restrict);
            });

            // Proveedor
            modelBuilder.Entity<Proveedor>(entity =>
            {
                entity.ToTable("Proveedores");
                entity.HasKey(e => e.ProveedorID);
                entity.Property(e => e.NombreProveedor).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => e.CUIT).IsUnique();
            });

            // Producto
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("Productos");
                entity.HasKey(e => e.ProductoID);
                entity.Property(e => e.CodigoBarras).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.CodigoBarras).IsUnique();
            });

            // OrdenCompra
            modelBuilder.Entity<OrdenCompra>(entity =>
            {
                entity.ToTable("OrdenesCompra");
                entity.HasKey(e => e.OrdenCompraID);
                entity.Property(e => e.NumeroOrden).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.NumeroOrden).IsUnique();
                entity.HasOne(e => e.Proveedor).WithMany().HasForeignKey(e => e.ProveedorID).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.UsuarioCreador).WithMany().HasForeignKey(e => e.UsuarioCreadorID).OnDelete(DeleteBehavior.Restrict);
            });

            // OrdenCompraDetalle
            modelBuilder.Entity<OrdenCompraDetalle>(entity =>
            {
                entity.ToTable("OrdenesCompraDetalle");
                entity.HasKey(e => e.DetalleID);
                entity.HasOne(e => e.OrdenCompra).WithMany(o => o.Detalles).HasForeignKey(e => e.OrdenCompraID).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Producto).WithMany().HasForeignKey(e => e.ProductoID).OnDelete(DeleteBehavior.Restrict);
            });

            // AuditoriaRecepcion
            modelBuilder.Entity<AuditoriaRecepcion>(entity =>
            {
                entity.ToTable("AuditoriasRecepcion");
                entity.HasKey(e => e.AuditoriaID);
                entity.Property(e => e.NumeroAuditoria).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.NumeroAuditoria).IsUnique();
                entity.HasOne(e => e.OrdenCompra).WithMany().HasForeignKey(e => e.OrdenCompraID).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Proveedor).WithMany().HasForeignKey(e => e.ProveedorID).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.UsuarioAuditor).WithMany().HasForeignKey(e => e.UsuarioAuditorID).OnDelete(DeleteBehavior.Restrict);
            });

            // AuditoriaDetalle
            modelBuilder.Entity<AuditoriaDetalle>(entity =>
            {
                entity.ToTable("AuditoriaDetalle");
                entity.HasKey(e => e.DetalleAuditoriaID);
                entity.HasOne(e => e.Auditoria).WithMany(a => a.Detalles).HasForeignKey(e => e.AuditoriaID).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Producto).WithMany().HasForeignKey(e => e.ProductoID).OnDelete(DeleteBehavior.Restrict);
            });

            // Evidencia
            modelBuilder.Entity<Evidencia>(entity =>
            {
                entity.ToTable("Evidencias");
                entity.HasKey(e => e.EvidenciaID);
                entity.HasOne(e => e.Auditoria).WithMany(a => a.Evidencias).HasForeignKey(e => e.AuditoriaID).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.DetalleAuditoria).WithMany().HasForeignKey(e => e.DetalleAuditoriaID).OnDelete(DeleteBehavior.Restrict);
            });

            // Incidencia
            modelBuilder.Entity<Incidencia>(entity =>
            {
                entity.ToTable("Incidencias");
                entity.HasKey(e => e.IncidenciaID);
                entity.HasOne(e => e.Auditoria).WithMany(a => a.Incidencias).HasForeignKey(e => e.AuditoriaID).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.UsuarioReporta).WithMany().HasForeignKey(e => e.UsuarioReportaID).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.UsuarioResponsable).WithMany().HasForeignKey(e => e.UsuarioResponsableID).OnDelete(DeleteBehavior.Restrict);
            });

            // AuditoriaLog
            modelBuilder.Entity<AuditoriaLog>(entity =>
            {
                entity.ToTable("AuditoriaLog");
                entity.HasKey(e => e.LogID);
                entity.HasOne(e => e.Usuario).WithMany().HasForeignKey(e => e.UsuarioID).OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
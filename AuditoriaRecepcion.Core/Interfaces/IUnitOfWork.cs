using AuditoriaRecepcion.Core.Entities;

namespace AuditoriaRecepcion.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Usuario> Usuarios { get; }
        IRepository<Proveedor> Proveedores { get; }
        IRepository<Producto> Productos { get; }
        IRepository<Auditoria> Auditorias { get; }
        IRepository<ProductoAuditoria> ProductosAuditoria { get; }
        IRepository<Incidencia> Incidencias { get; }
        IRepository<Evidencia> Evidencias { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
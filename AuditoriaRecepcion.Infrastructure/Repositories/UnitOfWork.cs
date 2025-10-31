using Microsoft.EntityFrameworkCore.Storage;
using AuditoriaRecepcion.Core.Entities;
using AuditoriaRecepcion.Core.Interfaces;
using AuditoriaRecepcion.Infrastructure.Data;

namespace AuditoriaRecepcion.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Usuarios = new Repository<Usuario>(_context);
            Proveedores = new Repository<Proveedor>(_context);
            Productos = new Repository<Producto>(_context);
            Auditorias = new Repository<Auditoria>(_context);
            ProductosAuditoria = new Repository<ProductoAuditoria>(_context);
            Incidencias = new Repository<Incidencia>(_context);
            Evidencias = new Repository<Evidencia>(_context);
        }

        public IRepository<Usuario> Usuarios { get; private set; }
        public IRepository<Proveedor> Proveedores { get; private set; }
        public IRepository<Producto> Productos { get; private set; }
        public IRepository<Auditoria> Auditorias { get; private set; }
        public IRepository<ProductoAuditoria> ProductosAuditoria { get; private set; }
        public IRepository<Incidencia> Incidencias { get; private set; }
        public IRepository<Evidencia> Evidencias { get; private set; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                    await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
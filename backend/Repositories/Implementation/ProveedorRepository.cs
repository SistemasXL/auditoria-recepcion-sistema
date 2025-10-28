using AuditoriaRecepcion.Data;
using AuditoriaRecepcion.Models;
using AuditoriaRecepcion.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuditoriaRecepcion.Repositories.Implementation
{
    public class ProveedorRepository : BaseRepository<Proveedor>, IProveedorRepository
    {
        public ProveedorRepository(AuditoriaRecepcionContext context) : base(context)
        {
        }

        public async Task<Proveedor> GetByCuitAsync(string cuit)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.CUIT == cuit);
        }

        public async Task<IEnumerable<Proveedor>> GetByProvinciaAsync(string provincia)
        {
            // Provincia no existe en el modelo actual
            return await _dbSet
                .Where(p => p.Activo)
                .OrderBy(p => p.NombreProveedor)
                .ToListAsync();
        }

        public async Task<IEnumerable<Proveedor>> SearchAsync(string busqueda)
        {
            if (string.IsNullOrWhiteSpace(busqueda))
                return await GetAllAsync();

            return await _dbSet
                .Where(p =>
                    p.NombreProveedor.Contains(busqueda) ||
                    p.NombreProveedor.Contains(busqueda) ||
                    p.CUIT.Contains(busqueda) ||
                    p.Email.Contains(busqueda))
                .OrderBy(p => p.NombreProveedor)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetProvinciasAsync()
        {
            // Provincia no existe en el modelo actual
            return await Task.FromResult(new List<string>());
        }

        public async Task<bool> ExisteCuitAsync(string cuit, int? excludeId = null)
        {
            var query = _dbSet.Where(p => p.CUIT == cuit);

            if (excludeId.HasValue)
                query = query.Where(p => p.ProveedorID != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}
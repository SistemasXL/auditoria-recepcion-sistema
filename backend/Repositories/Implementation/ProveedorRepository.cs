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
            return await _dbSet
                .Where(p => p.Provincia == provincia && p.Activo)
                .OrderBy(p => p.RazonSocial)
                .ToListAsync();
        }

        public async Task<IEnumerable<Proveedor>> SearchAsync(string busqueda)
        {
            if (string.IsNullOrWhiteSpace(busqueda))
                return await GetAllAsync();

            return await _dbSet
                .Where(p =>
                    p.RazonSocial.Contains(busqueda) ||
                    p.NombreFantasia.Contains(busqueda) ||
                    p.CUIT.Contains(busqueda) ||
                    p.Email.Contains(busqueda))
                .OrderBy(p => p.RazonSocial)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetProvinciasAsync()
        {
            return await _dbSet
                .Where(p => !string.IsNullOrEmpty(p.Provincia))
                .Select(p => p.Provincia)
                .Distinct()
                .OrderBy(prov => prov)
                .ToListAsync();
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
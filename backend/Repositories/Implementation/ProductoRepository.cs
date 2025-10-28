using AuditoriaRecepcion.Data;
using AuditoriaRecepcion.Models;
using AuditoriaRecepcion.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuditoriaRecepcion.Repositories.Implementation
{
    public class ProductoRepository : BaseRepository<Producto>, IProductoRepository
    {
        public ProductoRepository(AuditoriaRecepcionContext context) : base(context)
        {
        }

        public async Task<Producto> GetBySKUAsync(string sku)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.CodigoInterno == sku);
        }

        public async Task<Producto> GetByCodigoBarrasAsync(string codigoBarras)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.CodigoBarras == codigoBarras);
        }

        public async Task<IEnumerable<Producto>> GetByCategoriaAsync(string categoria)
        {
            return await _dbSet
                .Where(p => p.Categoria == categoria && p.Activo)
                .OrderBy(p => p.Descripcion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Producto>> SearchAsync(string busqueda)
        {
            if (string.IsNullOrWhiteSpace(busqueda))
                return await GetAllAsync();

            return await _dbSet
                .Where(p =>
                    p.CodigoInterno.Contains(busqueda) ||
                    p.Descripcion.Contains(busqueda) ||
                    p.Descripcion.Contains(busqueda) ||
                    p.CodigoBarras.Contains(busqueda))
                .OrderBy(p => p.Descripcion)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetCategoriasAsync()
        {
            return await _dbSet
                .Where(p => !string.IsNullOrEmpty(p.Categoria))
                .Select(p => p.Categoria)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public async Task<bool> ExisteSKUAsync(string sku, int? excludeId = null)
        {
            var query = _dbSet.Where(p => p.CodigoInterno == sku);

            if (excludeId.HasValue)
                query = query.Where(p => p.ProductoID != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> ExisteCodigoBarrasAsync(string codigoBarras, int? excludeId = null)
        {
            if (string.IsNullOrEmpty(codigoBarras))
                return false;

            var query = _dbSet.Where(p => p.CodigoBarras == codigoBarras);

            if (excludeId.HasValue)
                query = query.Where(p => p.ProductoID != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}
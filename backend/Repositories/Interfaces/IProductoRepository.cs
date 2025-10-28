using AuditoriaRecepcion.Models;

namespace AuditoriaRecepcion.Repositories.Interfaces
{
    public interface IProductoRepository : IRepository<Producto>
    {
        Task<Producto> GetBySKUAsync(string sku);
        Task<Producto> GetByCodigoBarrasAsync(string codigoBarras);
        Task<IEnumerable<Producto>> GetByCategoriaAsync(string categoria);
        Task<IEnumerable<Producto>> SearchAsync(string busqueda);
        Task<IEnumerable<string>> GetCategoriasAsync();
        Task<bool> ExisteSKUAsync(string sku, int? excludeId = null);
        Task<bool> ExisteCodigoBarrasAsync(string codigoBarras, int? excludeId = null);
    }
}
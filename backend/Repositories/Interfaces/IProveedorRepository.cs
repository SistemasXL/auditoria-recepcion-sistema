using AuditoriaRecepcion.Models;

namespace AuditoriaRecepcion.Repositories.Interfaces
{
    public interface IProveedorRepository : IRepository<Proveedor>
    {
        Task<Proveedor> GetByCuitAsync(string cuit);
        Task<IEnumerable<Proveedor>> GetByProvinciaAsync(string provincia);
        Task<IEnumerable<Proveedor>> SearchAsync(string busqueda);
        Task<IEnumerable<string>> GetProvinciasAsync();
        Task<bool> ExisteCuitAsync(string cuit, int? excludeId = null);
    }
}
using AuditoriaRecepcion.Models;

namespace AuditoriaRecepcion.Repositories.Interfaces
{
    public interface IAuditoriaRepository : IRepository<Models.AuditoriaRecepcion>
    {
        Task<Models.AuditoriaRecepcion> GetAuditoriaCompletaAsync(int id);
        Task<IEnumerable<Models.AuditoriaRecepcion>> GetAuditoriasByProveedorAsync(int proveedorId, DateTime? fechaDesde, DateTime? fechaHasta);
        Task<IEnumerable<Models.AuditoriaRecepcion>> GetAuditoriasAbiertasAsync();
        Task<IEnumerable<Models.AuditoriaRecepcion>> GetAuditoriasVencidasAsync(int diasVencimiento = 7);
    }
}

using AuditoriaRecepcion.Models;

namespace AuditoriaRecepcion.Repositories.Interfaces
{
    public interface IIncidenciaRepository : IRepository<Incidencia>
    {
        Task<IEnumerable<Incidencia>> GetByAuditoriaAsync(int auditoriaId);
        Task<IEnumerable<Incidencia>> GetByProveedorAsync(int proveedorId, DateTime? fechaDesde, DateTime? fechaHasta);
        Task<IEnumerable<Incidencia>> GetByProductoAsync(int productoId, DateTime? fechaDesde, DateTime? fechaHasta);
        Task<IEnumerable<Incidencia>> GetByUsuarioAsignadoAsync(int usuarioId, string estado = null);
        Task<IEnumerable<Incidencia>> GetPendientesAsync();
        Task<IEnumerable<Incidencia>> GetPorVencerAsync(int horasLimite = 48);
        Task<Dictionary<string, int>> GetDistribucionPorTipoAsync(DateTime? fechaDesde, DateTime? fechaHasta);
        Task<Dictionary<string, int>> GetDistribucionPorEstadoAsync(DateTime? fechaDesde, DateTime? fechaHasta);
    }
}
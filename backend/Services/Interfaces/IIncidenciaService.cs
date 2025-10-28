using AuditoriaRecepcion.DTOs.Incidencia;
using AuditoriaRecepcion.DTOs.Common;

namespace AuditoriaRecepcion.Services.Interfaces
{
    public interface IIncidenciaService
    {
        Task<PaginatedResult<IncidenciaDTO>> GetIncidenciasAsync(IncidenciaFiltroDTO filtro);
        Task<IncidenciaDetalleDTO> GetIncidenciaByIdAsync(int id);
        Task<IncidenciaDTO> CreateIncidenciaAsync(CreateIncidenciaDTO dto, int userId);
        Task<IncidenciaDTO> UpdateIncidenciaAsync(int id, UpdateIncidenciaDTO dto, int userId);
        Task CambiarEstadoAsync(int id, string estadoResolucion, string observaciones, int userId);
        Task<List<IncidenciaDTO>> GetIncidenciasPendientesByUsuarioAsync(int userId);
        Task<ResumenIncidenciasDTO> GetResumenIncidenciasAsync(DateTime? fechaDesde, DateTime? fechaHasta);
        Task AsignarIncidenciaAsync(int incidenciaId, int usuarioAsignadoId, int userId);
    }
}
using AuditoriaRecepcion.DTOs.Reporte;
using AuditoriaRecepcion.DTOs.Common;

namespace AuditoriaRecepcion.Services.Interfaces
{
    public interface IReporteService
    {
        // Generación de reportes
        Task<byte[]> GenerarReporteAuditoriaPDFAsync(int auditoriaId);
        Task<byte[]> GenerarReporteAuditoriaExcelAsync(int auditoriaId);
        Task<byte[]> GenerarReporteConsolidadoExcelAsync(ReporteConsolidadoRequestDTO request);
        Task<byte[]> GenerarReporteIncidenciasExcelAsync(ReporteIncidenciasRequestDTO request);
        Task<byte[]> GenerarReporteDesempenoProveedoresAsync(ReporteProveedoresRequestDTO request);
        Task<byte[]> GenerarReporteKPIsPDFAsync(ReporteKPIsRequestDTO request);
        Task<byte[]> GenerarReportePersonalizadoAsync(ReportePersonalizadoRequestDTO request);
        
        // Historial y programación
        Task<PaginatedResult<ReporteHistorialDTO>> GetHistorialReportesAsync(ReporteHistorialFiltroDTO filtro);
        Task<ReporteProgramadoDTO> ProgramarReporteAsync(ProgramarReporteDTO dto, int userId);
        Task<ReporteProgramadoDTO> GetReporteProgramadoByIdAsync(int id);
        Task CancelarReporteProgramadoAsync(int id);
    }
}
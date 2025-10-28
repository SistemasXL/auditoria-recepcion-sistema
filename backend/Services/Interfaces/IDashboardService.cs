using AuditoriaRecepcion.DTOs.Dashboard;

namespace AuditoriaRecepcion.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<KPIsGeneralesDTO> GetKPIsGeneralesAsync(DateTime? fechaDesde, DateTime? fechaHasta);
        Task<MetricasAuditoriasDTO> GetMetricasAuditoriasAsync(DateTime? fechaDesde, DateTime? fechaHasta, string agrupacion);
        Task<MetricasIncidenciasDTO> GetMetricasIncidenciasAsync(DateTime? fechaDesde, DateTime? fechaHasta);
        Task<List<TopProveedorIncidenciasDTO>> GetTopProveedoresIncidenciasAsync(DateTime? fechaDesde, DateTime? fechaHasta, int top);
        Task<List<TopProductoIncidenciasDTO>> GetTopProductosIncidenciasAsync(DateTime? fechaDesde, DateTime? fechaHasta, int top);
        Task<List<DistribucionIncidenciasDTO>> GetDistribucionIncidenciasAsync(DateTime? fechaDesde, DateTime? fechaHasta);
        Task<List<TendenciaAuditoriasDTO>> GetTendenciaAuditoriasAsync(DateTime? fechaDesde, DateTime? fechaHasta, string agrupacion);
        Task<MetricasTiempoResolucionDTO> GetMetricasTiempoResolucionAsync(DateTime? fechaDesde, DateTime? fechaHasta);
        Task<ResumenDiarioDTO> GetResumenDiarioAsync(DateTime fecha);
        Task<List<AlertaDTO>> GetAlertasPendientesAsync(int userId);
        Task<DashboardPersonalizadoDTO> GetDashboardPersonalizadoAsync(int userId, string userRole);
        Task<ComparativaPeriodosDTO> GetComparativaPeriodosAsync(ComparativaPeriodosRequestDTO request);
        Task<List<RendimientoOperadorDTO>> GetRendimientoOperadoresAsync(DateTime? fechaDesde, DateTime? fechaHasta);
    }
}
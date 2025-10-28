namespace AuditoriaRecepcion.DTOs.Dashboard
{
    public class DashboardPersonalizadoDTO
    {
        public string Rol { get; set; }
        public string NombreUsuario { get; set; }
        public KPIsGeneralesDTO KPIs { get; set; }
        public ResumenDiarioDTO ResumenHoy { get; set; }
        public List<AlertaDTO> AlertasPendientes { get; set; }
        public List<TareasPendientesDTO> TareasPendientes { get; set; }
        public List<TendenciaAuditoriasDTO> TendenciaUltimos30Dias { get; set; }
        public List<TopProveedorIncidenciasDTO> TopProveedoresProblematicos { get; set; }
        public Dictionary<string, object> WidgetsPersonalizados { get; set; }
    }
}
namespace AuditoriaRecepcion.DTOs.Reporte
{
    public class ReporteIncidenciasRequestDTO
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int? ProveedorID { get; set; }
        public string TipoIncidencia { get; set; }
        public string EstadoIncidencia { get; set; }
        public string Severidad { get; set; }
        public int? UsuarioResponsableID { get; set; }
    }
}

namespace AuditoriaRecepcion.DTOs.Reporte
{
    public class ReporteHistorialFiltroDTO
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string TipoReporte { get; set; }
        public string Formato { get; set; }
        public int? UsuarioId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
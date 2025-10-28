namespace AuditoriaRecepcion.DTOs.Incidencia
{
    public class IncidenciaFiltroDTO
    {
        public string TipoIncidencia { get; set; }
        public string EstadoResolucion { get; set; }
        public string Prioridad { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int? AuditoriaId { get; set; }
        public int? ProveedorId { get; set; }
        public int? ProductoId { get; set; }
        public int? UsuarioAsignadoId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
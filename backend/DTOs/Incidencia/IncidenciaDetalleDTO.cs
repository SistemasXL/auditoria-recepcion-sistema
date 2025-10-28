namespace AuditoriaRecepcion.DTOs.Incidencia
{
    public class IncidenciaDetalleDTO
    {
        public int IncidenciaID { get; set; }
        public AuditoriaDTO Auditoria { get; set; }
        public DetalleAuditoriaDTO DetalleAuditoria { get; set; }
        public ProductoDTO Producto { get; set; }
        public string TipoIncidencia { get; set; }
        public string Descripcion { get; set; }
        public string EstadoResolucion { get; set; }
        public string AccionCorrectiva { get; set; }
        public string Prioridad { get; set; }
        public UsuarioDTO UsuarioReporto { get; set; }
        public UsuarioDTO UsuarioAsignado { get; set; }
        public DateTime FechaDeteccion { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public List<EvidenciaDTO> Evidencias { get; set; }
        public List<ComentarioIncidenciaDTO> Comentarios { get; set; }
    }
}
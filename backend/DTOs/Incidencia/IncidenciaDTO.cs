namespace AuditoriaRecepcion.DTOs.Incidencia
{
    public class IncidenciaDTO
    {
        public int IncidenciaID { get; set; }
        public int AuditoriaID { get; set; }
        public string NumeroAuditoria { get; set; }
        public int? DetalleAuditoriaID { get; set; }
        public int? ProductoID { get; set; }
        public string ProductoNombre { get; set; }
        public string TipoIncidencia { get; set; }
        public string Descripcion { get; set; }
        public string EstadoResolucion { get; set; }
        public int? UsuarioAsignadoID { get; set; }
        public string UsuarioAsignadoNombre { get; set; }
        public DateTime FechaDeteccion { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public int CantidadEvidencias { get; set; }
        public string Prioridad { get; set; }
    }
}
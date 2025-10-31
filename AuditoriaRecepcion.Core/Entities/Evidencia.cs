namespace AuditoriaRecepcion.Core.Entities
{
    public class Evidencia : BaseEntity
    {
        public int AuditoriaId { get; set; }
        public int? ProductoAuditoriaId { get; set; }
        public int? IncidenciaId { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string RutaArchivo { get; set; } = string.Empty;
        public string TipoArchivo { get; set; } = string.Empty;
        public long TamanoBytes { get; set; }
        public string? Descripcion { get; set; }

        // Navegación
        public Auditoria Auditoria { get; set; } = null!;
        public ProductoAuditoria? ProductoAuditoria { get; set; }
        public Incidencia? Incidencia { get; set; }
    }
}
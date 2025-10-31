using AuditoriaRecepcion.Core.Enums;

namespace AuditoriaRecepcion.Core.Entities
{
    public class Incidencia : BaseEntity
    {
        public string NumeroIncidencia { get; set; } = string.Empty;
        public int AuditoriaId { get; set; }
        public int? ProductoAuditoriaId { get; set; }
        public TipoIncidencia Tipo { get; set; }
        public SeveridadIncidencia Severidad { get; set; }
        public EstadoIncidencia Estado { get; set; } = EstadoIncidencia.Abierta;
        public string Descripcion { get; set; } = string.Empty;
        public string? AccionCorrectiva { get; set; }
        public string? Resolucion { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public DateTime? FechaCierre { get; set; }
        public int? UsuarioResolucionId { get; set; }

        // Navegación
        public Auditoria Auditoria { get; set; } = null!;
        public ProductoAuditoria? ProductoAuditoria { get; set; }
        public Usuario? UsuarioResolucion { get; set; }
    }
}
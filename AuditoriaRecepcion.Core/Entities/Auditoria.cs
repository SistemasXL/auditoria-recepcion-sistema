using AuditoriaRecepcion.Core.Enums;

namespace AuditoriaRecepcion.Core.Entities
{
    public class Auditoria : BaseEntity
    {
        public string NumeroAuditoria { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public int ProveedorId { get; set; }
        public string OrdenCompra { get; set; } = string.Empty;
        public EstadoAuditoria Estado { get; set; } = EstadoAuditoria.Borrador;
        public string? Observaciones { get; set; }
        public int UsuarioCreadorId { get; set; }
        public DateTime? FechaFinalizacion { get; set; }
        public DateTime? FechaCierre { get; set; }

        // Navegación
        public Proveedor Proveedor { get; set; } = null!;
        public Usuario UsuarioCreador { get; set; } = null!;
        public ICollection<ProductoAuditoria> Productos { get; set; } = new List<ProductoAuditoria>();
        public ICollection<Incidencia> Incidencias { get; set; } = new List<Incidencia>();
        public ICollection<Evidencia> Evidencias { get; set; } = new List<Evidencia>();
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditoriaRecepcion.Models.Entities
{
    [Table("AuditoriasRecepcion")]
    public class AuditoriaRecepcion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string NumeroAuditoria { get; set; } = string.Empty;

        [Required]
        public int ProveedorId { get; set; }

        [Required]
        public DateTime FechaRecepcion { get; set; }

        [Required]
        public DateTime FechaAuditoria { get; set; } = DateTime.Now;

        [Required]
        public int UsuarioCreadorId { get; set; }

        public int? UsuarioAprobadorId { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Pendiente";

        [StringLength(1000)]
        public string Observaciones { get; set; } = string.Empty;

        [StringLength(1000)]
        public string ObservacionesAprobacion { get; set; } = string.Empty;

        public DateTime? FechaAprobacion { get; set; }

        public bool Completada { get; set; } = false;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaUltimaModificacion { get; set; }

        // Propiedades de navegación
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; } = null!;

        [ForeignKey("UsuarioCreadorId")]
        public virtual Usuario UsuarioCreador { get; set; } = null!;

        [ForeignKey("UsuarioAprobadorId")]
        public virtual Usuario? UsuarioAprobador { get; set; }

        public virtual ICollection<DetalleAuditoria> Detalles { get; set; } = new List<DetalleAuditoria>();
        public virtual ICollection<Incidencia> Incidencias { get; set; } = new List<Incidencia>();
        public virtual ICollection<Evidencia> Evidencias { get; set; } = new List<Evidencia>();
    }
}
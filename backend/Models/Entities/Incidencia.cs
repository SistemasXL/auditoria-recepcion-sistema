using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditoriaRecepcion.Models.Entities
{
    [Table("Incidencias")]
    public class Incidencia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string NumeroIncidencia { get; set; } = string.Empty;

        [Required]
        public int AuditoriaId { get; set; }

        public int? DetalleAuditoriaId { get; set; }

        public int? ProductoId { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoIncidencia { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Severidad { get; set; } = "Media";

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Abierta";

        [Required]
        public int UsuarioReportaId { get; set; }

        public int? UsuarioAsignadoId { get; set; }

        [StringLength(2000)]
        public string AccionesCorrectivas { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaAsignacion { get; set; }

        public DateTime? FechaResolucion { get; set; }

        public DateTime? FechaUltimaModificacion { get; set; }

        // Propiedades de navegación
        [ForeignKey("AuditoriaId")]
        public virtual AuditoriaRecepcion Auditoria { get; set; } = null!;

        [ForeignKey("DetalleAuditoriaId")]
        public virtual DetalleAuditoria? DetalleAuditoria { get; set; }

        [ForeignKey("ProductoId")]
        public virtual Producto? Producto { get; set; }

        [ForeignKey("UsuarioReportaId")]
        public virtual Usuario UsuarioReporta { get; set; } = null!;

        [ForeignKey("UsuarioAsignadoId")]
        public virtual Usuario? UsuarioAsignado { get; set; }

        public virtual ICollection<Evidencia> Evidencias { get; set; } = new List<Evidencia>();
    }
}
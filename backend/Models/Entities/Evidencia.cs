using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditoriaRecepcion.Models.Entities
{
    [Table("Evidencias")]
    public class Evidencia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string RutaArchivo { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string TipoArchivo { get; set; } = string.Empty;

        [Required]
        public long TamanoArchivo { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoEvidencia { get; set; } = "Foto";

        public int? AuditoriaId { get; set; }

        public int? IncidenciaId { get; set; }

        [StringLength(1000)]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public int UsuarioId { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Propiedades de navegación
        [ForeignKey("AuditoriaId")]
        public virtual AuditoriaRecepcion? Auditoria { get; set; }

        [ForeignKey("IncidenciaId")]
        public virtual Incidencia? Incidencia { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } = null!;
    }
}
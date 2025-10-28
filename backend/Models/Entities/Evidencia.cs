using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditoriaRecepcion.Models
{
    [Table("Evidencias")]
    public class Evidencia
    {
        [Key]
        public int EvidenciaID { get; set; }

        [Required]
        public int AuditoriaID { get; set; }

        public int? DetalleAuditoriaID { get; set; }

        [Required]
        [MaxLength(10)]
        public string TipoArchivo { get; set; }

        [Required]
        [MaxLength(500)]
        public string RutaArchivo { get; set; }

        [Required]
        [MaxLength(255)]
        public string NombreArchivo { get; set; }

        public int? TamañoKB { get; set; }

        public DateTime FechaSubida { get; set; } = DateTime.Now;

        [ForeignKey("AuditoriaID")]
        public virtual AuditoriaRecepcion Auditoria { get; set; }

        [ForeignKey("DetalleAuditoriaID")]
        public virtual AuditoriaDetalle DetalleAuditoria { get; set; }
    }
}

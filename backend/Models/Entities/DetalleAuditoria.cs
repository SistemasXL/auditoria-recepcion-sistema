using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditoriaRecepcion.Models.Entities
{
    [Table("DetallesAuditoria")]
    public class DetalleAuditoria
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AuditoriaId { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CantidadEsperada { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CantidadRecibida { get; set; }

        [StringLength(50)]
        public string EstadoProducto { get; set; } = "Conforme";

        [StringLength(1000)]
        public string Observaciones { get; set; } = string.Empty;

        public bool Conforme { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Propiedades de navegación
        [ForeignKey("AuditoriaId")]
        public virtual AuditoriaRecepcion Auditoria { get; set; } = null!;

        [ForeignKey("ProductoId")]
        public virtual Producto Producto { get; set; } = null!;

        public virtual ICollection<Incidencia> Incidencias { get; set; } = new List<Incidencia>();
    }
}
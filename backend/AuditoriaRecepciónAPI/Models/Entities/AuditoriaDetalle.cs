using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditoriaRecepcionAPI.Models.Entities
{
    [Table("AuditoriaDetalle")]
    public class AuditoriaDetalle
    {
        [Key]
        public int DetalleAuditoriaID { get; set; }

        [Required]
        public int AuditoriaID { get; set; }

        [Required]
        public int ProductoID { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal CantidadRecibida { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? CantidadEsperada { get; set; }

        [MaxLength(50)]
        public string TipoIncidencia { get; set; } = "SinIncidencia";

        [MaxLength(20)]
        public string EstadoItem { get; set; } = "Conforme";

        [MaxLength(500)]
        public string Observaciones { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [ForeignKey("AuditoriaID")]
        public virtual AuditoriaRecepcion Auditoria { get; set; }

        [ForeignKey("ProductoID")]
        public virtual Producto Producto { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditoriaRecepcion.Models
{
    [Table("OrdenesCompra")]
    public class OrdenCompra
    {
        [Key]
        public int OrdenCompraID { get; set; }

        [Required]
        [MaxLength(50)]
        public string NumeroOrden { get; set; }

        [Required]
        public int ProveedorID { get; set; }

        public DateTime FechaEmision { get; set; }

        public DateTime? FechaEntregaEsperada { get; set; }

        [MaxLength(20)]
        public string EstadoOrden { get; set; } = "Pendiente";

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MontoTotal { get; set; }

        public int? UsuarioCreadorID { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaModificacion { get; set; }

        [ForeignKey("ProveedorID")]
        public virtual Proveedor Proveedor { get; set; }

        [ForeignKey("UsuarioCreadorID")]
        public virtual Usuario UsuarioCreador { get; set; }

        public virtual ICollection<OrdenCompraDetalle> Detalles { get; set; }
    }
}

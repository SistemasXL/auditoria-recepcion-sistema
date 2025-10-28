using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditoriaRecepcion.Models
{
    [Table("OrdenesCompraDetalle")]
    public class OrdenCompraDetalle
    {
        [Key]
        public int DetalleID { get; set; }

        [Required]
        public int OrdenCompraID { get; set; }

        [Required]
        public int ProductoID { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal CantidadEsperada { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PrecioUnitario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Subtotal { get; set; }

        [ForeignKey("OrdenCompraID")]
        public virtual OrdenCompra OrdenCompra { get; set; }

        [ForeignKey("ProductoID")]
        public virtual Producto Producto { get; set; }
    }
}

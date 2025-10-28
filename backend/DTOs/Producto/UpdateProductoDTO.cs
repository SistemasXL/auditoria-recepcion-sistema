using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Producto
{
    public class UpdateProductoDTO
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(200)]
        public string Nombre { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        [StringLength(100)]
        public string CodigoBarras { get; set; }

        [StringLength(100)]
        public string Categoria { get; set; }

        [Required(ErrorMessage = "La unidad de medida es requerida")]
        [StringLength(20)]
        public string UnidadMedida { get; set; }

        [Range(0.001, double.MaxValue)]
        public decimal? PesoUnitario { get; set; }
    }
}
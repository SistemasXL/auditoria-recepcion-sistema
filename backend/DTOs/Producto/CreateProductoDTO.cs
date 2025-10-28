using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Producto
{
    public class CreateProductoDTO
    {
        [Required(ErrorMessage = "El SKU es requerido")]
        [StringLength(50, ErrorMessage = "El SKU no puede exceder 50 caracteres")]
        public string SKU { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string Nombre { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string Descripcion { get; set; }

        [StringLength(100, ErrorMessage = "El código de barras no puede exceder 100 caracteres")]
        public string CodigoBarras { get; set; }

        [StringLength(100, ErrorMessage = "La categoría no puede exceder 100 caracteres")]
        public string Categoria { get; set; }

        [Required(ErrorMessage = "La unidad de medida es requerida")]
        [StringLength(20, ErrorMessage = "La unidad de medida no puede exceder 20 caracteres")]
        public string UnidadMedida { get; set; }

        [Range(0.001, double.MaxValue, ErrorMessage = "El peso debe ser mayor a 0")]
        public decimal? PesoUnitario { get; set; }
    }
}
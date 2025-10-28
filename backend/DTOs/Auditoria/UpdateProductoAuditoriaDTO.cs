using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Auditoria
{
    public class UpdateProductoAuditoriaDTO
    {
        [Required(ErrorMessage = "La cantidad esperada es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad esperada debe ser mayor a 0")]
        public int CantidadEsperada { get; set; }

        [Required(ErrorMessage = "La cantidad recibida es requerida")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad recibida no puede ser negativa")]
        public int CantidadRecibida { get; set; }

        [Required(ErrorMessage = "El estado del producto es requerido")]
        [RegularExpression("^(Bueno|Dañado|Faltante|Excedente)$")]
        public string EstadoProducto { get; set; }

        [StringLength(500)]
        public string Observaciones { get; set; }
    }
}
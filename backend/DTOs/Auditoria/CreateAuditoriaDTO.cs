using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Auditoria
{
    public class CreateAuditoriaDTO
    {
        [Required(ErrorMessage = "La fecha de recepción es requerida")]
        public DateTime FechaRecepcion { get; set; }

        [Required(ErrorMessage = "El proveedor es requerido")]
        public int ProveedorID { get; set; }

        [Required(ErrorMessage = "El número de orden de compra es requerido")]
        [StringLength(50, ErrorMessage = "El número de orden de compra no puede exceder 50 caracteres")]
        public string NumeroOrdenCompra { get; set; }

        [StringLength(50, ErrorMessage = "El número de remito no puede exceder 50 caracteres")]
        public string NumeroRemito { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
        public string Observaciones { get; set; }
    }
}
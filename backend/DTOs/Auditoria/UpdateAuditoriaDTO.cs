using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Auditoria
{
    public class UpdateAuditoriaDTO
    {
        [Required(ErrorMessage = "La fecha de recepción es requerida")]
        public DateTime FechaRecepcion { get; set; }

        [Required(ErrorMessage = "El proveedor es requerido")]
        public int ProveedorID { get; set; }

        [Required(ErrorMessage = "El número de orden de compra es requerido")]
        [StringLength(50)]
        public string NumeroOrdenCompra { get; set; }

        [StringLength(50)]
        public string NumeroRemito { get; set; }

        [StringLength(500)]
        public string Observaciones { get; set; }
    }
}
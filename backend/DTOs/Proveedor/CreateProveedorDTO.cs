using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Proveedor
{
    public class CreateProveedorDTO
    {
        [Required(ErrorMessage = "La razón social es requerida")]
        [StringLength(200, ErrorMessage = "La razón social no puede exceder 200 caracteres")]
        public string RazonSocial { get; set; }

        [StringLength(200, ErrorMessage = "El nombre de fantasía no puede exceder 200 caracteres")]
        public string NombreFantasia { get; set; }

        [Required(ErrorMessage = "El CUIT es requerido")]
        [StringLength(20, ErrorMessage = "El CUIT no puede exceder 20 caracteres")]
        [RegularExpression(@"^\d{2}-\d{8}-\d{1}$", ErrorMessage = "Formato de CUIT inválido (XX-XXXXXXXX-X)")]
        public string CUIT { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100)]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Teléfono inválido")]
        [StringLength(20)]
        public string Telefono { get; set; }

        [StringLength(200)]
        public string Direccion { get; set; }

        [StringLength(100)]
        public string Ciudad { get; set; }

        [StringLength(100)]
        public string Provincia { get; set; }

        [StringLength(10)]
        public string CodigoPostal { get; set; }

        [StringLength(100)]
        public string PersonaContacto { get; set; }
    }
}
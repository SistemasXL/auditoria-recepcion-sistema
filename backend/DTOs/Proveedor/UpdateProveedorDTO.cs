using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Proveedor
{
    public class UpdateProveedorDTO
    {
        [Required(ErrorMessage = "La razón social es requerida")]
        [StringLength(200)]
        public string RazonSocial { get; set; }

        [StringLength(200)]
        public string NombreFantasia { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Phone]
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
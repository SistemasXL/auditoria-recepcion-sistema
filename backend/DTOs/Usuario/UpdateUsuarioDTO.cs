using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Usuario
{
    public class UpdateUsuarioDTO
    {
        [Required(ErrorMessage = "El nombre completo es requerido")]
        [StringLength(200)]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Phone]
        [StringLength(20)]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El rol es requerido")]
        [RegularExpression("^(Operador|JefeLogistica|Comprador|Administrador)$")]
        public string Rol { get; set; }
    }
}
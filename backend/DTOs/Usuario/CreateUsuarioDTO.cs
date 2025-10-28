using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Usuario
{
    public class CreateUsuarioDTO
    {
        [Required(ErrorMessage = "El usuario es requerido")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "El usuario debe tener entre 4 y 50 caracteres")]
        [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "El usuario solo puede contener letras, números, puntos, guiones y guiones bajos")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "El nombre completo es requerido")]
        [StringLength(200, ErrorMessage = "El nombre completo no puede exceder 200 caracteres")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100)]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Teléfono inválido")]
        [StringLength(20)]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El rol es requerido")]
        [RegularExpression("^(Operador|JefeLogistica|Comprador|Administrador)$", ErrorMessage = "Rol inválido")]
        public string Rol { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]",
            ErrorMessage = "La contraseña debe contener al menos una mayúscula, una minúscula, un número y un carácter especial")]
        public string Contrasena { get; set; }

        public bool Activo { get; set; } = true;
    }
}
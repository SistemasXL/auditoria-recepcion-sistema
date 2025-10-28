using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Auth
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "El usuario es requerido")]
        [StringLength(50, ErrorMessage = "El usuario no puede exceder 50 caracteres")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Contrasena { get; set; }

        public bool RecordarSesion { get; set; } = false;
    }
}
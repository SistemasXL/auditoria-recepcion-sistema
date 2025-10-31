using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.Application.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "El usuario es requerido")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; } = string.Empty;
    }
}
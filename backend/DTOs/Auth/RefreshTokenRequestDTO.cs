using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Auth
{
    public class RefreshTokenRequestDTO
    {
        [Required(ErrorMessage = "El refresh token es requerido")]
        public string RefreshToken { get; set; }
    }
}
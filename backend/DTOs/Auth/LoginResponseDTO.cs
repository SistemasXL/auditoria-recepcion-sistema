namespace AuditoriaRecepcion.DTOs.Auth
{
    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiracion { get; set; }
        public UsuarioDTO Usuario { get; set; }
    }
}
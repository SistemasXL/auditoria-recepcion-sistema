namespace AuditoriaRecepcion.Application.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UsuarioDto Usuario { get; set; } = null!;
    }

    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public int RolId { get; set; }
        public string Rol { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}
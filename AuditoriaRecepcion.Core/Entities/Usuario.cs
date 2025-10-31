using AuditoriaRecepcion.Core.Enums;

namespace AuditoriaRecepcion.Core.Entities
{
    public class Usuario : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public int RolId { get; set; }
        public RolUsuario Rol { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        // Navegación
        public ICollection<Auditoria> Auditorias { get; set; } = new List<Auditoria>();
    }
}
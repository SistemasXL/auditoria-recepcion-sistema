using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditoriaRecepcion.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int UsuarioID { get; set; }

        [Required]
        [MaxLength(50)]
        public string NombreUsuario { get; set; }

        [Required]
        [MaxLength(100)]
        public string NombreCompleto { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string ContrasenaHash { get; set; }

        [Required]
        public int RolID { get; set; }

        [MaxLength(20)]
        public string Rol { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaModificacion { get; set; }

        public DateTime? UltimoAcceso { get; set; }

        public int IntentosLoginFallidos { get; set; } = 0;

        public DateTime? FechaBloqueo { get; set; }

        public string RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiry { get; set; }

        [ForeignKey("RolID")]
        public virtual Rol RolNavigation { get; set; }
    }
}

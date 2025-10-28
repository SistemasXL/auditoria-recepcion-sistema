using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Usuario
{
    public class UpdatePerfilDTO
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

        public Dictionary<string, object> Preferencias { get; set; }
    }
}
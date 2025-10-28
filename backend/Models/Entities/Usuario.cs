using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditoriaRecepcion.Models.Entities
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        [StringLength(50)]
        public string Rol { get; set; } = "Operador";

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaUltimoAcceso { get; set; }

        // Propiedades de navegación
        public virtual ICollection<AuditoriaRecepcion> AuditoriasCreadas { get; set; } = new List<AuditoriaRecepcion>();
        public virtual ICollection<AuditoriaRecepcion> AuditoriasAprobadas { get; set; } = new List<AuditoriaRecepcion>();
        public virtual ICollection<Incidencia> IncidenciasCreadas { get; set; } = new List<Incidencia>();
        public virtual ICollection<Incidencia> IncidenciasAsignadas { get; set; } = new List<Incidencia>();
    }
}
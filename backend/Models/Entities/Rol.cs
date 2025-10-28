using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditoriaRecepcion.Models
{
    [Table("Roles")]
    public class Rol
    {
        [Key]
        public int RolID { get; set; }

        [Required]
        [MaxLength(50)]
        public string NombreRol { get; set; }

        [MaxLength(255)]
        public string Descripcion { get; set; }

        public string Permisos { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public bool Activo { get; set; } = true;
    }
}

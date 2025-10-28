using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditoriaRecepcionAPI.Models.Entities
{
    [Table("Productos")]
    public class Producto
    {
        [Key]
        public int ProductoID { get; set; }

        [Required]
        [MaxLength(50)]
        public string CodigoBarras { get; set; }

        [MaxLength(50)]
        public string CodigoInterno { get; set; }

        [Required]
        [MaxLength(255)]
        public string Descripcion { get; set; }

        [Required]
        [MaxLength(20)]
        public string UnidadMedida { get; set; }

        [MaxLength(50)]
        public string Categoria { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaModificacion { get; set; }
    }
}
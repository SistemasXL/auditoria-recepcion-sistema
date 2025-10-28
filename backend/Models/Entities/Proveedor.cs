using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditoriaRecepcion.Models
{
    [Table("Proveedores")]
    public class Proveedor
    {
        [Key]
        public int ProveedorID { get; set; }

        [Required]
        [MaxLength(200)]
        public string NombreProveedor { get; set; }

        [MaxLength(13)]
        public string CUIT { get; set; }

        [MaxLength(100)]
        public string Contacto { get; set; }

        [MaxLength(20)]
        public string Telefono { get; set; }

        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(255)]
        public string Direccion { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaModificacion { get; set; }
    }
}

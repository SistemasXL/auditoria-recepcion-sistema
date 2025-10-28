using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditoriaRecepcion.Models.Entities
{
    [Table("Productos")]
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public int ProveedorId { get; set; }

        [StringLength(50)]
        public string UnidadMedida { get; set; } = "UN";

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PrecioUnitario { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaUltimaModificacion { get; set; }

        // Propiedades de navegación
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; } = null!;

        public virtual ICollection<DetalleAuditoria> DetallesAuditoria { get; set; } = new List<DetalleAuditoria>();
        public virtual ICollection<Incidencia> Incidencias { get; set; } = new List<Incidencia>();
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditoriaRecepcionAPI.Models.Entities
{
    [Table("AuditoriasRecepcion")]
    public class AuditoriaRecepcion
    {
        [Key]
        public int AuditoriaID { get; set; }

        [Required]
        [MaxLength(50)]
        public string NumeroAuditoria { get; set; }

        public int? OrdenCompraID { get; set; }

        [Required]
        public int ProveedorID { get; set; }

        [Required]
        public int UsuarioAuditorID { get; set; }

        public DateTime FechaInicio { get; set; } = DateTime.Now;

        public DateTime? FechaFinalizacion { get; set; }

        [MaxLength(20)]
        public string EstadoAuditoria { get; set; } = "En Proceso";

        public string ObservacionesGenerales { get; set; }

        public bool RequiereAtencion { get; set; } = false;

        [ForeignKey("OrdenCompraID")]
        public virtual OrdenCompra OrdenCompra { get; set; }

        [ForeignKey("ProveedorID")]
        public virtual Proveedor Proveedor { get; set; }

        [ForeignKey("UsuarioAuditorID")]
        public virtual Usuario UsuarioAuditor { get; set; }

        public virtual ICollection<AuditoriaDetalle> Detalles { get; set; }
        public virtual ICollection<Evidencia> Evidencias { get; set; }
        public virtual ICollection<Incidencia> Incidencias { get; set; }
    }
}
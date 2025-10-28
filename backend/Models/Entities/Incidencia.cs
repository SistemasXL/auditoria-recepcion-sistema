using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditoriaRecepcion.Models
{
    [Table("Incidencias")]
    public class Incidencia
    {
        [Key]
        public int IncidenciaID { get; set; }

        [Required]
        public int AuditoriaID { get; set; }

        [Required]
        [MaxLength(50)]
        public string TipoIncidencia { get; set; }

        [MaxLength(20)]
        public string Severidad { get; set; } = "Media";

        [Required]
        public string Descripcion { get; set; }

        [MaxLength(20)]
        public string EstadoIncidencia { get; set; } = "Abierta";

        [Required]
        public int UsuarioReportaID { get; set; }

        public int? UsuarioResponsableID { get; set; }

        public DateTime FechaReporte { get; set; } = DateTime.Now;

        public DateTime? FechaResolucion { get; set; }

        public string ComentariosResolucion { get; set; }

        [ForeignKey("AuditoriaID")]
        public virtual AuditoriaRecepcion Auditoria { get; set; }

        [ForeignKey("UsuarioReportaID")]
        public virtual Usuario UsuarioReporta { get; set; }

        [ForeignKey("UsuarioResponsableID")]
        public virtual Usuario UsuarioResponsable { get; set; }
    }
}

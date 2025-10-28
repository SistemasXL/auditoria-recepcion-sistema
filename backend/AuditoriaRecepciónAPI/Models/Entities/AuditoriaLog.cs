using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditoriaRecepcionAPI.Models.Entities
{
    [Table("AuditoriaLog")]
    public class AuditoriaLog
    {
        [Key]
        public long LogID { get; set; }

        public int? UsuarioID { get; set; }

        [Required]
        [MaxLength(50)]
        public string TipoAccion { get; set; }

        [MaxLength(50)]
        public string TablaAfectada { get; set; }

        public int? RegistroID { get; set; }

        public string ValoresAnteriores { get; set; }

        public string ValoresNuevos { get; set; }

        [MaxLength(45)]
        public string DireccionIP { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.Now;

        [ForeignKey("UsuarioID")]
        public virtual Usuario Usuario { get; set; }
    }
}
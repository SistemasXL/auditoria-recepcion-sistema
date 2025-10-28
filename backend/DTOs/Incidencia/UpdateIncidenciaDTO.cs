using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Incidencia
{
    public class UpdateIncidenciaDTO
    {
        [Required(ErrorMessage = "El tipo de incidencia es requerido")]
        [RegularExpression("^(Faltante|Excedente|Dañado|Defectuoso|Incorrecto|DocumentacionIncompleta|Otro)$")]
        public string TipoIncidencia { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(1000)]
        public string Descripcion { get; set; }

        [StringLength(1000)]
        public string AccionCorrectiva { get; set; }

        [RegularExpression("^(Baja|Media|Alta|Critica)$")]
        public string Prioridad { get; set; }

        public int? UsuarioAsignadoID { get; set; }
    }
}
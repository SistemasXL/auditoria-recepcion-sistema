using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Incidencia
{
    public class CreateIncidenciaDTO
    {
        [Required(ErrorMessage = "La auditoría es requerida")]
        public int AuditoriaID { get; set; }

        public int? DetalleAuditoriaID { get; set; }

        public int? ProductoID { get; set; }

        [Required(ErrorMessage = "El tipo de incidencia es requerido")]
        [RegularExpression("^(Faltante|Excedente|Dañado|Defectuoso|Incorrecto|DocumentacionIncompleta|Otro)$",
            ErrorMessage = "Tipo de incidencia inválido")]
        public string TipoIncidencia { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string Descripcion { get; set; }

        [RegularExpression("^(Baja|Media|Alta|Critica)$", ErrorMessage = "Prioridad inválida")]
        public string Prioridad { get; set; } = "Media";

        public int? UsuarioAsignadoID { get; set; }
    }
}
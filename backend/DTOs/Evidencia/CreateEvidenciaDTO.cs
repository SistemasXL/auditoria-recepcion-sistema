using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Evidencia
{
    public class CreateEvidenciaDTO
    {
        [Required(ErrorMessage = "La auditoría es requerida")]
        public int AuditoriaID { get; set; }

        public int? DetalleAuditoriaID { get; set; }

        public int? IncidenciaID { get; set; }

        [Required(ErrorMessage = "El tipo de evidencia es requerido")]
        [RegularExpression("^(Foto|Video|Documento)$", ErrorMessage = "Tipo de evidencia inválido")]
        public string TipoEvidencia { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string Descripcion { get; set; }
    }
}
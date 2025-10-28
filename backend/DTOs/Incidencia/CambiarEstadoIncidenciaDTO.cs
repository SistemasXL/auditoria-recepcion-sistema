using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Incidencia
{
    public class CambiarEstadoIncidenciaDTO
    {
        [Required(ErrorMessage = "El estado de resolución es requerido")]
        [RegularExpression("^(Pendiente|EnProceso|Resuelta|Rechazada)$",
            ErrorMessage = "Estado de resolución inválido")]
        public string EstadoResolucion { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
        public string Observaciones { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Reporte
{
    public class ProgramarReporteDTO
    {
        [Required(ErrorMessage = "El nombre del reporte es requerido")]
        [StringLength(200)]
        public string NombreReporte { get; set; }

        [Required(ErrorMessage = "El tipo de reporte es requerido")]
        public string TipoReporte { get; set; }

        [Required(ErrorMessage = "El formato es requerido")]
        [RegularExpression("^(PDF|Excel)$")]
        public string Formato { get; set; }

        [Required(ErrorMessage = "La frecuencia es requerida")]
        [RegularExpression("^(Diario|Semanal|Mensual)$", ErrorMessage = "Frecuencia inválida")]
        public string Frecuencia { get; set; }

        public int? DiaSemana { get; set; } // 1-7 para semanal
        public int? DiaMes { get; set; } // 1-31 para mensual

        [Required(ErrorMessage = "La hora de ejecución es requerida")]
        public TimeSpan HoraEjecucion { get; set; }

        [Required(ErrorMessage = "Debe especificar al menos un destinatario")]
        public List<string> EmailsDestinatarios { get; set; }

        public Dictionary<string, object> ParametrosReporte { get; set; }
        public bool Activo { get; set; } = true;
    }
}
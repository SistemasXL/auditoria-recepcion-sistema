using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Dashboard
{
    public class ComparativaPeriodosRequestDTO
    {
        [Required]
        public DateTime Periodo1Inicio { get; set; }
        
        [Required]
        public DateTime Periodo1Fin { get; set; }
        
        [Required]
        public DateTime Periodo2Inicio { get; set; }
        
        [Required]
        public DateTime Periodo2Fin { get; set; }
        
        public List<string> MetricasComparar { get; set; }
    }
}
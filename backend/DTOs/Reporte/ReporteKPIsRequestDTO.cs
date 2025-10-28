using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Reporte
{
    public class ReporteKPIsRequestDTO
    {
        [Required(ErrorMessage = "La fecha desde es requerida")]
        public DateTime FechaDesde { get; set; }

        [Required(ErrorMessage = "La fecha hasta es requerida")]
        public DateTime FechaHasta { get; set; }

        public string Agrupacion { get; set; } = "Mes"; // Dia, Semana, Mes
        public bool CompararConPeriodoAnterior { get; set; } = true;
        public List<string> KPIsSeleccionados { get; set; } // null = todos
    }
}
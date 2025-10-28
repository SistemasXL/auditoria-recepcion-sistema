using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Reporte
{
    public class ReporteProveedoresRequestDTO
    {
        [Required(ErrorMessage = "La fecha desde es requerida")]
        public DateTime FechaDesde { get; set; }

        [Required(ErrorMessage = "La fecha hasta es requerida")]
        public DateTime FechaHasta { get; set; }

        public List<int> ProveedoresIDs { get; set; }
        public bool IncluirRanking { get; set; } = true;
        public bool IncluirTendencias { get; set; } = true;
        public string OrdenarPor { get; set; } = "IncidenciasPorcentaje"; // IncidenciasPorcentaje, TiempoResolucion, CantidadRecepciones
    }
}
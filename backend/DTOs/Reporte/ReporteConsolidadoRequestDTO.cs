using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Reporte
{
    public class ReporteConsolidadoRequestDTO
    {
        [Required(ErrorMessage = "La fecha desde es requerida")]
        public DateTime FechaDesde { get; set; }

        [Required(ErrorMessage = "La fecha hasta es requerida")]
        public DateTime FechaHasta { get; set; }

        public List<int> ProveedoresIDs { get; set; }
        public List<string> Estados { get; set; }
        public bool IncluirDetalles { get; set; } = true;
        public bool IncluirIncidencias { get; set; } = true;
        public bool IncluirGraficos { get; set; } = true;
    }
}
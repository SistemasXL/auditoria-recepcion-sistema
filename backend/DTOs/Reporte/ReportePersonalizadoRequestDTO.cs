using System.ComponentModel.DataAnnotations;

namespace AuditoriaRecepcion.DTOs.Reporte
{
    public class ReportePersonalizadoRequestDTO
    {
        [Required(ErrorMessage = "El nombre del reporte es requerido")]
        [StringLength(200)]
        public string NombreReporte { get; set; }

        [Required(ErrorMessage = "El formato es requerido")]
        [RegularExpression("^(PDF|Excel)$", ErrorMessage = "Formato inválido")]
        public string Formato { get; set; }

        [Required(ErrorMessage = "La fecha desde es requerida")]
        public DateTime FechaDesde { get; set; }

        [Required(ErrorMessage = "La fecha hasta es requerida")]
        public DateTime FechaHasta { get; set; }

        [Required(ErrorMessage = "Debe seleccionar al menos una sección")]
        public List<string> SeccionesIncluidas { get; set; }
        // Opciones: Auditorias, Incidencias, Productos, Proveedores, KPIs, Tendencias

        public Dictionary<string, object> FiltrosAdicionales { get; set; }
        public bool IncluirGraficos { get; set; } = true;
        public bool IncluirDetalles { get; set; } = true;
    }
}
namespace AuditoriaRecepcion.DTOs.Dashboard
{
    public class MetricasIncidenciasDTO
    {
        public int TotalIncidencias { get; set; }
        public int IncidenciasPendientes { get; set; }
        public int IncidenciasEnProceso { get; set; }
        public int IncidenciasResueltas { get; set; }
        public int IncidenciasRechazadas { get; set; }
        public decimal TasaResolucion { get; set; }
        public decimal TiempoPromedioResolucion { get; set; }
        public decimal TiempoMedianoResolucion { get; set; }
        
        // Distribución por tipo
        public Dictionary<string, int> DistribucionPorTipo { get; set; }
        
        // Distribución por prioridad
        public Dictionary<string, int> DistribucionPorPrioridad { get; set; }
        
        // Serie temporal
        public List<SerieTemporalDTO> SerieTemporal { get; set; }
    }
}
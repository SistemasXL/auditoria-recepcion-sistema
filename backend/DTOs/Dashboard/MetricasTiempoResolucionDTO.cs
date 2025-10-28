namespace AuditoriaRecepcion.DTOs.Dashboard
{
    public class MetricasTiempoResolucionDTO
    {
        public decimal TiempoPromedio { get; set; } // En horas
        public decimal TiempoMediano { get; set; }
        public decimal TiempoMinimo { get; set; }
        public decimal TiempoMaximo { get; set; }
        public decimal DesviacionEstandar { get; set; }
        
        // Distribución por rangos de tiempo
        public Dictionary<string, int> DistribucionPorRango { get; set; }
        // Ej: "0-24h": 15, "24-48h": 8, "48-72h": 3, ">72h": 2
        
        // Por tipo de incidencia
        public Dictionary<string, decimal> TiempoPorTipoIncidencia { get; set; }
        
        // Por prioridad
        public Dictionary<string, decimal> TiempoPorPrioridad { get; set; }
    }
}
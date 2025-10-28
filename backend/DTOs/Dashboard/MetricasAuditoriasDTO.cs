namespace AuditoriaRecepcion.DTOs.Dashboard
{
    public class MetricasAuditoriasDTO
    {
        public int TotalAuditorias { get; set; }
        public int AuditoriasAbiertas { get; set; }
        public int AuditoriasCerradas { get; set; }
        public decimal PromedioProductosPorAuditoria { get; set; }
        public decimal PromedioTiempoCierre { get; set; } // En horas
        
        // Serie temporal
        public List<SerieTemporalDTO> SerieTemporal { get; set; }
        
        // Distribución por estado
        public Dictionary<string, int> DistribucionPorEstado { get; set; }
        
        // Top usuarios con más auditorías
        public List<TopUsuarioAuditoriasDTO> TopUsuarios { get; set; }
    }
}
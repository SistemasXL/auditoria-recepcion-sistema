namespace AuditoriaRecepcion.DTOs.Proveedor
{
    public class ProveedorEstadisticasDTO
    {
        public int ProveedorID { get; set; }
        public string RazonSocial { get; set; }
        public int TotalAuditorias { get; set; }
        public int TotalIncidencias { get; set; }
        public decimal PorcentajeIncidencias { get; set; }
        public int IncidenciasPendientes { get; set; }
        public int IncidenciasResueltas { get; set; }
        public decimal TiempoPromedioResolucion { get; set; } // En horas
        public DateTime? UltimaRecepcion { get; set; }
        
        // Distribución de tipos de incidencias
        public Dictionary<string, int> IncidenciasPorTipo { get; set; }
    }
}
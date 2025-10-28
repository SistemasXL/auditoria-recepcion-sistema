namespace AuditoriaRecepcion.DTOs.Incidencia
{
    public class ResumenIncidenciasDTO
    {
        public int TotalIncidencias { get; set; }
        public int Pendientes { get; set; }
        public int EnProceso { get; set; }
        public int Resueltas { get; set; }
        public int Rechazadas { get; set; }
        public decimal PorcentajeResolucion { get; set; }
        public decimal TiempoPromedioResolucion { get; set; } // En horas
        
        // Distribución por tipo
        public Dictionary<string, int> IncidenciasPorTipo { get; set; }
        
        // Distribución por prioridad
        public Dictionary<string, int> IncidenciasPorPrioridad { get; set; }
        
        // Top proveedores con más incidencias
        public List<TopProveedorIncidenciasDTO> TopProveedores { get; set; }
    }
}
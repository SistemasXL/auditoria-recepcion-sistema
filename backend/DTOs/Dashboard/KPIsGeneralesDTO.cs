namespace AuditoriaRecepcion.DTOs.Dashboard
{
    public class KPIsGeneralesDTO
    {
        // Auditorías
        public int TotalAuditorias { get; set; }
        public int AuditoriasAbiertas { get; set; }
        public int AuditoriasCerradas { get; set; }
        public decimal TasaCierre { get; set; }
        
        // Incidencias
        public int TotalIncidencias { get; set; }
        public int IncidenciasPendientes { get; set; }
        public int IncidenciasResueltas { get; set; }
        public decimal TasaResolucion { get; set; }
        public decimal TiempoPromedioResolucion { get; set; } // En horas
        
        // Productos
        public int TotalProductosAuditados { get; set; }
        public int ProductosConIncidencias { get; set; }
        public decimal PorcentajeProductosOK { get; set; }
        
        // Proveedores
        public int TotalProveedoresActivos { get; set; }
        public int ProveedoresConIncidencias { get; set; }
        
        // Comparación con período anterior
        public decimal CambioAuditorias { get; set; }
        public decimal CambioIncidencias { get; set; }
        public decimal CambioTiempoResolucion { get; set; }
    }
}
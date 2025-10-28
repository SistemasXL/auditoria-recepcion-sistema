namespace AuditoriaRecepcion.DTOs.Dashboard
{
    public class TendenciaAuditoriasDTO
    {
        public DateTime Fecha { get; set; }
        public string Periodo { get; set; } // Ej: "2025-01", "2025-W04", "2025-01-15"
        public int CantidadAuditorias { get; set; }
        public int CantidadIncidencias { get; set; }
        public decimal PorcentajeIncidencias { get; set; }
    }
}
namespace AuditoriaRecepcion.DTOs.Dashboard
{
    public class RendimientoOperadorDTO
    {
        public int UsuarioID { get; set; }
        public string NombreCompleto { get; set; }
        public string Usuario { get; set; }
        public int AuditoriasRealizadas { get; set; }
        public int IncidenciasDetectadas { get; set; }
        public decimal TiempoPromedioAuditoria { get; set; } // En minutos
        public decimal TasaDeteccionIncidencias { get; set; }
        public int EvidenciasSubidas { get; set; }
        public decimal CalificacionDesempeno { get; set; } // 0-100
        public string Categoria { get; set; } // Excelente, Bueno, Regular, Necesita Mejora
    }
}
namespace AuditoriaRecepcion.DTOs.Dashboard
{
    public class TopProveedorIncidenciasDTO
    {
        public int ProveedorID { get; set; }
        public string RazonSocial { get; set; }
        public int TotalAuditorias { get; set; }
        public int TotalIncidencias { get; set; }
        public decimal PorcentajeIncidencias { get; set; }
        public decimal TiempoPromedioResolucion { get; set; }
        public string Calificacion { get; set; } // Excelente, Bueno, Regular, Malo
    }
}
namespace AuditoriaRecepcion.DTOs.Usuario
{
    public class UsuarioDetalleDTO
    {
        public int UsuarioID { get; set; }
        public string Usuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Rol { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public string CreadoPorNombre { get; set; }
        
        // Estadísticas
        public int TotalAuditorias { get; set; }
        public int TotalIncidenciasDetectadas { get; set; }
        public int TotalIncidenciasAsignadas { get; set; }
        public int IncidenciasResueltasAsignadas { get; set; }
    }
}
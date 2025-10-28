namespace AuditoriaRecepcion.DTOs.Usuario
{
    public class UsuarioPerfilDTO
    {
        public int UsuarioID { get; set; }
        public string Usuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Rol { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        
        // Preferencias
        public Dictionary<string, object> Preferencias { get; set; }
        
        // Estadísticas personales
        public int TotalAuditoriasRealizadas { get; set; }
        public int TotalIncidenciasDetectadas { get; set; }
        public int AuditoriasMesActual { get; set; }
        public int IncidenciasPendientesAsignadas { get; set; }
    }
}
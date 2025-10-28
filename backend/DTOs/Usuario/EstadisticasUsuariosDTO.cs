namespace AuditoriaRecepcion.DTOs.Usuario
{
    public class EstadisticasUsuariosDTO
    {
        public int TotalUsuarios { get; set; }
        public int UsuariosActivos { get; set; }
        public int UsuariosInactivos { get; set; }
        public Dictionary<string, int> UsuariosPorRol { get; set; }
        public int UsuariosConectadosHoy { get; set; }
        public DateTime? UltimoUsuarioCreado { get; set; }
    }
}
namespace AuditoriaRecepcion.DTOs.Usuario
{
    public class UsuarioDTO
    {
        public int UsuarioID { get; set; }
        public string Usuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
        public bool Activo { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
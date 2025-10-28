namespace AuditoriaRecepcion.DTOs.Usuario
{
    public class UsuarioFiltroDTO
    {
        public string Busqueda { get; set; }
        public string Rol { get; set; }
        public bool? Activo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
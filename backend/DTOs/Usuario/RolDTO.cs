namespace AuditoriaRecepcion.DTOs.Usuario
{
    public class RolDTO
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public List<string> Permisos { get; set; }
    }
}
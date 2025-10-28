namespace AuditoriaRecepcion.DTOs.Incidencia
{
    public class ComentarioIncidenciaDTO
    {
        public int ComentarioID { get; set; }
        public int IncidenciaID { get; set; }
        public string Comentario { get; set; }
        public UsuarioDTO Usuario { get; set; }
        public DateTime FechaComentario { get; set; }
    }
}
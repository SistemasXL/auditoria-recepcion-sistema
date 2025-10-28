namespace AuditoriaRecepcion.DTOs.Usuario
{
    public class ActividadUsuarioDTO
    {
        public int ActividadID { get; set; }
        public string TipoAccion { get; set; }
        public string TablaAfectada { get; set; }
        public int? RegistroID { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaHora { get; set; }
        public string DireccionIP { get; set; }
    }
}
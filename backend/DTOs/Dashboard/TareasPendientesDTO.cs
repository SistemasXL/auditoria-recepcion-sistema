namespace AuditoriaRecepcion.DTOs.Dashboard
{
    public class TareasPendientesDTO
    {
        public int TareaID { get; set; }
        public string TipoTarea { get; set; }
        public string Descripcion { get; set; }
        public string Prioridad { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public bool Vencida { get; set; }
        public string UrlAccion { get; set; }
    }
}
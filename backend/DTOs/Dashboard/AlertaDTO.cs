namespace AuditoriaRecepcion.DTOs.Dashboard
{
    public class AlertaDTO
    {
        public int AlertaID { get; set; }
        public string TipoAlerta { get; set; } // Incidencia, AuditoriaVencida, ProveedorProblematico, etc.
        public string Prioridad { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaAlerta { get; set; }
        public bool Leida { get; set; }
        public string UrlAccion { get; set; }
        public Dictionary<string, object> DatosAdicionales { get; set; }
    }
}
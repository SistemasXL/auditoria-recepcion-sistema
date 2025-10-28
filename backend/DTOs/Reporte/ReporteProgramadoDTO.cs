namespace AuditoriaRecepcion.DTOs.Reporte
{
    public class ReporteProgramadoDTO
    {
        public int ReporteProgramadoID { get; set; }
        public string NombreReporte { get; set; }
        public string TipoReporte { get; set; }
        public string Formato { get; set; }
        public string Frecuencia { get; set; }
        public int? DiaSemana { get; set; }
        public int? DiaMes { get; set; }
        public TimeSpan HoraEjecucion { get; set; }
        public List<string> EmailsDestinatarios { get; set; }
        public bool Activo { get; set; }
        public DateTime? UltimaEjecucion { get; set; }
        public DateTime? ProximaEjecucion { get; set; }
        public string UsuarioCreacionNombre { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
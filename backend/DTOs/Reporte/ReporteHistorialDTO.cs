namespace AuditoriaRecepcion.DTOs.Reporte
{
    public class ReporteHistorialDTO
    {
        public int ReporteID { get; set; }
        public string NombreReporte { get; set; }
        public string TipoReporte { get; set; }
        public string Formato { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string UsuarioGeneracionNombre { get; set; }
        public long TamanoBytes { get; set; }
        public string TamanoLegible { get; set; }
        public string RutaArchivo { get; set; }
        public string UrlDescarga { get; set; }
        public DateTime? FechaExpiracion { get; set; }
    }
}
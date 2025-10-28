namespace AuditoriaRecepcion.DTOs.Evidencia
{
    public class EvidenciaDTO
    {
        public int EvidenciaID { get; set; }
        public int AuditoriaID { get; set; }
        public int? DetalleAuditoriaID { get; set; }
        public int? IncidenciaID { get; set; }
        public string TipoEvidencia { get; set; }
        public string NombreArchivo { get; set; }
        public string RutaArchivo { get; set; }
        public string TipoArchivo { get; set; }
        public long TamanoBytes { get; set; }
        public string TamanoLegible { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCarga { get; set; }
        public string UsuarioCargaNombre { get; set; }
        public string UrlDescarga { get; set; }
        public string UrlVisualizacion { get; set; }
    }
}
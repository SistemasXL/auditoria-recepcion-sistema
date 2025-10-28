namespace AuditoriaRecepcion.DTOs.Evidencia
{
    public class FileDownloadDTO
    {
        public byte[] FileBytes { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}
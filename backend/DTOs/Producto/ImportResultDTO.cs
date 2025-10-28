namespace AuditoriaRecepcion.DTOs.Producto
{
    public class ImportResultDTO
    {
        public int TotalRegistros { get; set; }
        public int Exitosos { get; set; }
        public int Fallidos { get; set; }
        public List<string> Errores { get; set; } = new List<string>();
        public List<ProductoDTO> ProductosImportados { get; set; } = new List<ProductoDTO>();
    }
}
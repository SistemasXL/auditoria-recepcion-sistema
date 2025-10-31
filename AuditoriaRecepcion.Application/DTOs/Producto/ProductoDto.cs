namespace AuditoriaRecepcion.Application.DTOs.Producto
{
    public class ProductoDto
    {
        public int Id { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string CodigoBarras { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public string UnidadMedida { get; set; } = string.Empty;
        public decimal? PrecioUnitario { get; set; }
        public bool Activo { get; set; }
    }
}
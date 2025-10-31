namespace AuditoriaRecepcion.Core.Entities
{
    public class Producto : BaseEntity
    {
        public string Sku { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string CodigoBarras { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public string UnidadMedida { get; set; } = string.Empty;
        public decimal? PrecioUnitario { get; set; }

        // Navegación
        public ICollection<ProductoAuditoria> ProductosAuditoria { get; set; } = new List<ProductoAuditoria>();
    }
}
namespace AuditoriaRecepcion.DTOs.Producto
{
    public class ProductoDTO
    {
        public int ProductoID { get; set; }
        public string SKU { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string CodigoBarras { get; set; }
        public string Categoria { get; set; }
        public string UnidadMedida { get; set; }
        public decimal? PesoUnitario { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
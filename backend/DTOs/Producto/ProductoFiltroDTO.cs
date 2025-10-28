namespace AuditoriaRecepcion.DTOs.Producto
{
    public class ProductoFiltroDTO
    {
        public string Busqueda { get; set; }
        public string Categoria { get; set; }
        public bool? Activo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
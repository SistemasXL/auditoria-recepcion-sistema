namespace AuditoriaRecepcion.DTOs.Proveedor
{
    public class ProveedorFiltroDTO
    {
        public string Busqueda { get; set; }
        public bool? Activo { get; set; }
        public string Provincia { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
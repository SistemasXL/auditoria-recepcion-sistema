namespace AuditoriaRecepcion.DTOs.Auditoria
{
    public class AuditoriaFiltroDTO
    {
        public string Estado { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int? UsuarioId { get; set; }
        public int? ProveedorId { get; set; }
        public string NumeroOrdenCompra { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
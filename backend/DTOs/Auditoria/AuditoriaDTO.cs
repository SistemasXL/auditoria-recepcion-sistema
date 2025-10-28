namespace AuditoriaRecepcion.DTOs.Auditoria
{
    public class AuditoriaDTO
    {
        public int AuditoriaID { get; set; }
        public string NumeroAuditoria { get; set; }
        public DateTime FechaRecepcion { get; set; }
        public int ProveedorID { get; set; }
        public string ProveedorNombre { get; set; }
        public string NumeroOrdenCompra { get; set; }
        public string NumeroRemito { get; set; }
        public string Estado { get; set; }
        public int CantidadProductos { get; set; }
        public int CantidadIncidencias { get; set; }
        public int UsuarioCreacionID { get; set; }
        public string UsuarioCreacionNombre { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string Observaciones { get; set; }
    }
}
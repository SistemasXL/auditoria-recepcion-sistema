namespace AuditoriaRecepcion.DTOs.Auditoria
{
    public class DetalleAuditoriaDTO
    {
        public int DetalleAuditoriaID { get; set; }
        public int AuditoriaID { get; set; }
        public int ProductoID { get; set; }
        public string ProductoNombre { get; set; }
        public string ProductoSKU { get; set; }
        public string ProductoCodigoBarras { get; set; }
        public int CantidadEsperada { get; set; }
        public int CantidadRecibida { get; set; }
        public int Diferencia { get; set; }
        public string EstadoProducto { get; set; }
        public string Observaciones { get; set; }
        public bool TieneIncidencias { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
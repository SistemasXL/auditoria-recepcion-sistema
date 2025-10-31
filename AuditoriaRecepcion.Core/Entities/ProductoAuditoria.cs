namespace AuditoriaRecepcion.Core.Entities
{
    public class ProductoAuditoria
    {
        public int Id { get; set; }
        public int AuditoriaId { get; set; }
        public int ProductoId { get; set; }
        public int CantidadEsperada { get; set; }
        public int CantidadRecibida { get; set; }
        public int CantidadDiferencia { get; set; }
        public string? Observaciones { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Navegación
        public Auditoria Auditoria { get; set; } = null!;
        public Producto Producto { get; set; } = null!;
    }
}
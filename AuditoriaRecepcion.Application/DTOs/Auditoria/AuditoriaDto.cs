using AuditoriaRecepcion.Core.Enums;

namespace AuditoriaRecepcion.Application.DTOs.Auditoria
{
    public class AuditoriaDto
    {
        public int Id { get; set; }
        public string NumeroAuditoria { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Hora { get; set; } = string.Empty;
        public ProveedorSimpleDto Proveedor { get; set; } = null!;
        public string OrdenCompra { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public int TotalProductos { get; set; }
        public bool TieneIncidencias { get; set; }
        public string UsuarioCreador { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaFinalizacion { get; set; }
        public DateTime? FechaCierre { get; set; }
    }

    public class CreateAuditoriaDto
    {
        public int ProveedorId { get; set; }
        public string OrdenCompra { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string? Observaciones { get; set; }
    }

    public class UpdateAuditoriaDto
    {
        public int? ProveedorId { get; set; }
        public string? OrdenCompra { get; set; }
        public DateTime? Fecha { get; set; }
        public TimeSpan? Hora { get; set; }
        public string? Observaciones { get; set; }
    }

    public class ProveedorSimpleDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
    }

    public class ProductoAuditoriaDto
    {
        public int Id { get; set; }
        public ProductoSimpleDto Producto { get; set; } = null!;
        public int CantidadEsperada { get; set; }
        public int CantidadRecibida { get; set; }
        public int CantidadDiferencia { get; set; }
        public string? Observaciones { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    public class ProductoSimpleDto
    {
        public int Id { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string CodigoBarras { get; set; } = string.Empty;
        public string UnidadMedida { get; set; } = string.Empty;
    }

    public class AgregarProductoDto
    {
        public int ProductoId { get; set; }
        public int CantidadEsperada { get; set; }
        public int CantidadRecibida { get; set; }
        public string? Observaciones { get; set; }
    }
}
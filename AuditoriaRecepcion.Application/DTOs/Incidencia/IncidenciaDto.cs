namespace AuditoriaRecepcion.Application.DTOs.Incidencia
{
    public class IncidenciaDto
    {
        public int Id { get; set; }
        public string NumeroIncidencia { get; set; } = string.Empty;
        public AuditoriaSimpleDto Auditoria { get; set; } = null!;
        public string Tipo { get; set; } = string.Empty;
        public string Severidad { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? AccionCorrectiva { get; set; }
        public string? Resolucion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public DateTime? FechaCierre { get; set; }
    }

    public class CreateIncidenciaDto
    {
        public int AuditoriaId { get; set; }
        public int? ProductoAuditoriaId { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Severidad { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? AccionCorrectiva { get; set; }
    }

    public class AuditoriaSimpleDto
    {
        public int Id { get; set; }
        public string NumeroAuditoria { get; set; } = string.Empty;
    }
}
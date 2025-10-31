namespace AuditoriaRecepcion.Core.Entities
{
    public class Proveedor : BaseEntity
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Contacto { get; set; }

        // Navegación
        public ICollection<Auditoria> Auditorias { get; set; } = new List<Auditoria>();
    }
}
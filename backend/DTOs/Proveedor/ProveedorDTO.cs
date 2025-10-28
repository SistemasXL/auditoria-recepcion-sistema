namespace AuditoriaRecepcion.DTOs.Proveedor
{
    public class ProveedorDTO
    {
        public int ProveedorID { get; set; }
        public string RazonSocial { get; set; }
        public string NombreFantasia { get; set; }
        public string CUIT { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string Provincia { get; set; }
        public string CodigoPostal { get; set; }
        public string PersonaContacto { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
namespace AuditoriaRecepcion.DTOs.Dashboard
{
    public class TopUsuarioAuditoriasDTO
    {
        public int UsuarioID { get; set; }
        public string NombreCompleto { get; set; }
        public int CantidadAuditorias { get; set; }
        public decimal Porcentaje { get; set; }
    }
}
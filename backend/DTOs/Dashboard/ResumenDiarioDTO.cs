namespace AuditoriaRecepcion.DTOs.Dashboard
{
    public class ResumenDiarioDTO
    {
        public DateTime Fecha { get; set; }
        public int AuditoriasCreadas { get; set; }
        public int AuditoriasCerradas { get; set; }
        public int AuditoriasAbiertas { get; set; }
        public int IncidenciasNuevas { get; set; }
        public int IncidenciasResueltas { get; set; }
        public int IncidenciasPendientes { get; set; }
        public int ProductosAuditados { get; set; }
        public int EvidenciasSubidas { get; set; }
        public List<string> AlertasImportantes { get; set; }
    }
}
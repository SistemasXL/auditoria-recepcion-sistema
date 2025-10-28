using AuditoriaRecepcion.DTOs.Proveedor;
using AuditoriaRecepcion.DTOs.Usuario;
using AuditoriaRecepcion.DTOs.Incidencia;
using AuditoriaRecepcion.DTOs.Evidencia;

namespace AuditoriaRecepcion.DTOs.Auditoria
{
    public class AuditoriaDetalleDTO
    {
        public int AuditoriaID { get; set; }
        public string NumeroAuditoria { get; set; }
        public DateTime FechaRecepcion { get; set; }
        public ProveedorDTO Proveedor { get; set; }
        public string NumeroOrdenCompra { get; set; }
        public string NumeroRemito { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public DateTime? FechaCierre { get; set; }
        public UsuarioDTO UsuarioCreacion { get; set; }
        public UsuarioDTO UsuarioModificacion { get; set; }
        
        public List<DetalleAuditoriaDTO> Detalles { get; set; }
        public List<IncidenciaDTO> Incidencias { get; set; }
        public List<EvidenciaDTO> Evidencias { get; set; }
    }
}
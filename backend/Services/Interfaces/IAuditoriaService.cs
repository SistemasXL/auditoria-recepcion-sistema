using AuditoriaRecepcion.DTOs.Auditoria;
using AuditoriaRecepcion.DTOs.Common;

namespace AuditoriaRecepcion.Services.Interfaces
{
    public interface IAuditoriaService
    {
        Task<PaginatedResult<AuditoriaDTO>> GetAuditoriasAsync(AuditoriaFiltroDTO filtro);
        Task<AuditoriaDetalleDTO> GetAuditoriaByIdAsync(int id);
        Task<AuditoriaDTO> CreateAuditoriaAsync(CreateAuditoriaDTO dto, int userId);
        Task<AuditoriaDTO> UpdateAuditoriaAsync(int id, UpdateAuditoriaDTO dto, int userId);
        Task CerrarAuditoriaAsync(int id, int userId);
        Task DeleteAuditoriaAsync(int id, int userId);
        
        // Gestión de productos en auditoría
        Task<DetalleAuditoriaDTO> AddProductoAsync(int auditoriaId, AddProductoAuditoriaDTO dto, int userId);
        Task<DetalleAuditoriaDTO> UpdateProductoAsync(int auditoriaId, int detalleId, UpdateProductoAuditoriaDTO dto, int userId);
        Task DeleteProductoAsync(int auditoriaId, int detalleId, int userId);
        
        // Validaciones y utilidades
        Task<bool> ValidarOrdenCompraAsync(string numeroOrdenCompra);
        Task<string> GenerarNumeroAuditoriaAsync();
    }
}
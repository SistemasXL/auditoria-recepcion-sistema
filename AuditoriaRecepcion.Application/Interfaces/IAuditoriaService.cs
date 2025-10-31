using AuditoriaRecepcion.Application.DTOs.Auditoria;
using AuditoriaRecepcion.Application.DTOs.Common;

namespace AuditoriaRecepcion.Application.Interfaces
{
    public interface IAuditoriaService
    {
        Task<PaginatedResponseDto<AuditoriaDto>> GetAuditoriasAsync(int page, int pageSize, string? search = null, string? estado = null);
        Task<AuditoriaDto> GetAuditoriaByIdAsync(int id);
        Task<AuditoriaDto> CreateAuditoriaAsync(CreateAuditoriaDto dto, int usuarioId);
        Task<AuditoriaDto> UpdateAuditoriaAsync(int id, UpdateAuditoriaDto dto);
        Task<AuditoriaDto> FinalizarAuditoriaAsync(int id);
        Task<AuditoriaDto> CerrarAuditoriaAsync(int id);
        Task<IEnumerable<ProductoAuditoriaDto>> GetProductosAuditoriaAsync(int auditoriaId);
        Task<ProductoAuditoriaDto> AgregarProductoAsync(int auditoriaId, AgregarProductoDto dto);
    }
}
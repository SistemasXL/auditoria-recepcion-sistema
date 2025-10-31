using AuditoriaRecepcion.Application.DTOs.Common;
using AuditoriaRecepcion.Application.DTOs.Producto;

namespace AuditoriaRecepcion.Application.Interfaces
{
    public interface IProductoService
    {
        Task<PaginatedResponseDto<ProductoDto>> GetProductosAsync(int page, int pageSize, string? search = null);
        Task<ProductoDto> GetProductoByIdAsync(int id);
        Task<ProductoDto> GetProductoByCodigoAsync(string codigo);
    }
}
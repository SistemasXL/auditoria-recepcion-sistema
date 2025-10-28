using AuditoriaRecepcion.DTOs.Producto;
using AuditoriaRecepcion.DTOs.Common;
using Microsoft.AspNetCore.Http;

namespace AuditoriaRecepcion.Services.Interfaces
{
    public interface IProductoService
    {
        Task<PaginatedResult<ProductoDTO>> GetProductosAsync(ProductoFiltroDTO filtro);
        Task<ProductoDTO> GetProductoByIdAsync(int id);
        Task<ProductoDTO> GetProductoByCodigoBarrasAsync(string codigoBarras);
        Task<ProductoDTO> GetProductoBySKUAsync(string sku);
        Task<ProductoDTO> CreateProductoAsync(CreateProductoDTO dto, int userId);
        Task<ProductoDTO> UpdateProductoAsync(int id, UpdateProductoDTO dto, int userId);
        Task ToggleProductoStatusAsync(int id, int userId);
        Task DeleteProductoAsync(int id, int userId);
        
        // Importación/Exportación
        Task<ImportResultDTO> ImportProductosAsync(IFormFile file, int userId);
        Task<byte[]> ExportProductosAsync(string busqueda = null);
    }
}
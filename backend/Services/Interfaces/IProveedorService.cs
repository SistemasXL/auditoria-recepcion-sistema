using AuditoriaRecepcion.DTOs.Proveedor;
using AuditoriaRecepcion.DTOs.Common;

namespace AuditoriaRecepcion.Services.Interfaces
{
    public interface IProveedorService
    {
        Task<PaginatedResult<ProveedorDTO>> GetProveedoresAsync(ProveedorFiltroDTO filtro);
        Task<ProveedorDTO> GetProveedorByIdAsync(int id);
        Task<ProveedorDTO> GetProveedorByCuitAsync(string cuit);
        Task<ProveedorDTO> CreateProveedorAsync(CreateProveedorDTO dto, int userId);
        Task<ProveedorDTO> UpdateProveedorAsync(int id, UpdateProveedorDTO dto, int userId);
        Task ToggleProveedorStatusAsync(int id, int userId);
        Task<ProveedorEstadisticasDTO> GetEstadisticasProveedorAsync(int id, DateTime? fechaDesde, DateTime? fechaHasta);
        Task<bool> ValidarCuitAsync(string cuit);
    }
}
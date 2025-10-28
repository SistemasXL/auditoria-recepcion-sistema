using AuditoriaRecepcion.DTOs.Usuario;
using AuditoriaRecepcion.DTOs.Common;

namespace AuditoriaRecepcion.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<PaginatedResult<UsuarioDTO>> GetUsuariosAsync(UsuarioFiltroDTO filtro);
        Task<UsuarioDetalleDTO> GetUsuarioByIdAsync(int id);
        Task<UsuarioDTO> CreateUsuarioAsync(CreateUsuarioDTO dto, int adminId);
        Task<UsuarioDTO> UpdateUsuarioAsync(int id, UpdateUsuarioDTO dto, int adminId);
        Task ToggleUsuarioStatusAsync(int id, int adminId);
        Task<ResetPasswordResponseDTO> ResetPasswordAsync(int id, int adminId);
        Task<UsuarioPerfilDTO> GetPerfilUsuarioAsync(int userId);
        Task<UsuarioPerfilDTO> UpdatePerfilUsuarioAsync(int userId, UpdatePerfilDTO dto);
        Task<List<ActividadUsuarioDTO>> GetActividadUsuarioAsync(int userId, DateTime? fechaDesde, DateTime? fechaHasta, int limit);
        Task<List<RolDTO>> GetRolesDisponiblesAsync();
        Task<EstadisticasUsuariosDTO> GetEstadisticasUsuariosAsync();
        Task<bool> IsUsernameAvailableAsync(string username);
        Task<bool> IsEmailAvailableAsync(string email);
    }
}
using AuditoriaRecepcion.DTOs.Auth;
using AuditoriaRecepcion.DTOs.Usuario;

namespace AuditoriaRecepcion.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request);
        Task<LoginResponseDTO> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(int userId);
        Task ChangePasswordAsync(int userId, ChangePasswordDTO dto);
        Task<UsuarioDTO> GetUserInfoAsync(int userId);
        Task<bool> ValidateTokenAsync(string token);
        string GenerateJwtToken(int userId, string username, string role);
        string GenerateRefreshToken();
    }
}
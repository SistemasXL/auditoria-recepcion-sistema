using AuditoriaRecepcion.Application.DTOs.Auth;

namespace AuditoriaRecepcion.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<LoginResponseDto> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(int userId);
    }
}
using Microsoft.AspNetCore.Mvc;
using AuditoriaRecepcion.Application.DTOs.Auth;
using AuditoriaRecepcion.Application.DTOs.Common;
using AuditoriaRecepcion.Application.Interfaces;

namespace AuditoriaRecepcion.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponseDto<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(ApiResponseDto<LoginResponseDto>.SuccessResponse(result, "Login exitoso"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponseDto<LoginResponseDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<LoginResponseDto>.ErrorResponse("Error interno del servidor"));
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponseDto<LoginResponseDto>>> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(refreshToken);
                return Ok(ApiResponseDto<LoginResponseDto>.SuccessResponse(result, "Token renovado"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponseDto<LoginResponseDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<LoginResponseDto>.ErrorResponse("Error interno del servidor"));
            }
        }
    }
}
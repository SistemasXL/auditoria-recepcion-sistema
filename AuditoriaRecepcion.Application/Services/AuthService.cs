using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using AuditoriaRecepcion.Application.DTOs.Auth;
using AuditoriaRecepcion.Application.Interfaces;
using AuditoriaRecepcion.Core.Interfaces;
using AuditoriaRecepcion.Core.Entities;

namespace AuditoriaRecepcion.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            // Buscar usuario por username
            var usuarios = await _unitOfWork.Usuarios.FindAsync(u => u.Username == request.Username);
            var usuario = usuarios.FirstOrDefault();

            if (usuario == null || !VerifyPassword(request.Password, usuario.PasswordHash))
            {
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");
            }

            if (!usuario.Activo)
            {
                throw new UnauthorizedAccessException("Usuario inactivo");
            }

            // Generar tokens
            var token = GenerateJwtToken(usuario);
            var refreshToken = GenerateRefreshToken();

            // Actualizar refresh token en BD
            usuario.RefreshToken = refreshToken;
            usuario.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            usuario.UltimoAcceso = DateTime.UtcNow;
            await _unitOfWork.Usuarios.UpdateAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            return new LoginResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:ExpirationMinutes"])),
                Usuario = new UsuarioDto
                {
                    Id = usuario.Id,
                    Username = usuario.Username,
                    Email = usuario.Email,
                    NombreCompleto = usuario.NombreCompleto,
                    RolId = usuario.RolId,
                    Rol = usuario.Rol.ToString(),
                    Activo = usuario.Activo
                }
            };
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var usuarios = await _unitOfWork.Usuarios.FindAsync(u => 
                u.RefreshToken == refreshToken && 
                u.RefreshTokenExpiry > DateTime.UtcNow);
            
            var usuario = usuarios.FirstOrDefault();

            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Token inválido o expirado");
            }

            var newToken = GenerateJwtToken(usuario);
            var newRefreshToken = GenerateRefreshToken();

            usuario.RefreshToken = newRefreshToken;
            usuario.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _unitOfWork.Usuarios.UpdateAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            return new LoginResponseDto
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:ExpirationMinutes"])),
                Usuario = new UsuarioDto
                {
                    Id = usuario.Id,
                    Username = usuario.Username,
                    Email = usuario.Email,
                    NombreCompleto = usuario.NombreCompleto,
                    RolId = usuario.RolId,
                    Rol = usuario.Rol.ToString(),
                    Activo = usuario.Activo
                }
            };
        }

        public async Task LogoutAsync(int userId)
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(userId);
            if (usuario != null)
            {
                usuario.RefreshToken = null;
                usuario.RefreshTokenExpiry = null;
                await _unitOfWork.Usuarios.UpdateAsync(usuario);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
                new Claim("RolId", usuario.RolId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(jwtSettings["ExpirationMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
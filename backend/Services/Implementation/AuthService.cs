using AuditoriaRecepcion.Data;
using AuditoriaRecepcion.DTOs.Auth;
using AuditoriaRecepcion.DTOs.Usuario;
using AuditoriaRecepcion.Models;
using AuditoriaRecepcion.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuditoriaRecepcion.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly AuditoriaRecepcionContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            AuditoriaRecepcionContext context,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            try
            {
                // Buscar usuario
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.NombreUsuario == request.Usuario && u.Activo);

                if (usuario == null)
                {
                    _logger.LogWarning("Intento de login fallido para usuario: {Usuario}", request.Usuario);
                    throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");
                }

                // Verificar contraseña
                if (!VerifyPassword(request.Contrasena, usuario.ContrasenaHash))
                {
                    _logger.LogWarning("Contraseña incorrecta para usuario: {Usuario}", request.Usuario);
                    throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");
                }

                // Generar tokens
                var token = GenerateJwtToken(usuario.UsuarioID, usuario.NombreUsuario, usuario.Rol);
                var refreshToken = GenerateRefreshToken();

                // Guardar refresh token
                usuario.RefreshToken = refreshToken;
                usuario.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                usuario.UltimoAcceso = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Login exitoso para usuario: {Usuario}", request.Usuario);

                return new LoginResponseDTO
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    Expiracion = DateTime.UtcNow.AddHours(8),
                    Usuario = MapToUsuarioDTO(usuario)
                };
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en login para usuario: {Usuario}", request.Usuario);
                throw new Exception("Error al procesar el login");
            }
        }

        public async Task<LoginResponseDTO> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.Activo);

                if (usuario == null || usuario.RefreshTokenExpiry < DateTime.UtcNow)
                {
                    throw new UnauthorizedAccessException("Refresh token inválido o expirado");
                }

                // Generar nuevos tokens
                var token = GenerateJwtToken(usuario.UsuarioID, usuario.NombreUsuario, usuario.Rol);
                var newRefreshToken = GenerateRefreshToken();

                usuario.RefreshToken = newRefreshToken;
                usuario.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                usuario.UltimoAcceso = DateTime.Now;

                await _context.SaveChangesAsync();

                return new LoginResponseDTO
                {
                    Token = token,
                    RefreshToken = newRefreshToken,
                    Expiracion = DateTime.UtcNow.AddHours(8),
                    Usuario = MapToUsuarioDTO(usuario)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en refresh token");
                throw;
            }
        }

        public async Task LogoutAsync(int userId)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(userId);
                if (usuario != null)
                {
                    usuario.RefreshToken = null;
                    usuario.RefreshTokenExpiry = null;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en logout para usuario: {UserId}", userId);
                throw;
            }
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordDTO dto)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(userId);
                if (usuario == null)
                    throw new InvalidOperationException("Usuario no encontrado");

                // Verificar contraseña actual
                if (!VerifyPassword(dto.ContrasenaActual, usuario.ContrasenaHash))
                    throw new UnauthorizedAccessException("Contraseña actual incorrecta");

                // Actualizar contraseña
                usuario.ContrasenaHash = HashPassword(dto.NuevaContrasena);
                usuario.FechaModificacion = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Contraseña cambiada para usuario: {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contraseña para usuario: {UserId}", userId);
                throw;
            }
        }

        public async Task<UsuarioDTO> GetUserInfoAsync(int userId)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UsuarioID == userId);

                if (usuario == null)
                    throw new InvalidOperationException("Usuario no encontrado");

                return MapToUsuarioDTO(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener información del usuario: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GenerateJwtToken(int userId, string username, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("UserId", userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        // Métodos privados auxiliares
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        private UsuarioDTO MapToUsuarioDTO(Usuario usuario)
        {
            return new UsuarioDTO
            {
                UsuarioID = usuario.UsuarioID,
                Usuario = usuario.NombreUsuario,
                NombreCompleto = usuario.NombreCompleto,
                Email = usuario.Email,
                Rol = usuario.Rol,
                Activo = usuario.Activo,
                UltimoAcceso = usuario.UltimoAcceso,
                FechaCreacion = usuario.FechaCreacion
            };
        }
    }
}
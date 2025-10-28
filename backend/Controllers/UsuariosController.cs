using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuditoriaRecepcion.DTOs;
using AuditoriaRecepcion.Services.Interfaces;

namespace AuditoriaRecepcion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(IUsuarioService usuarioService, ILogger<UsuariosController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todos los usuarios con filtros
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(PaginatedResult<UsuarioDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResult<UsuarioDTO>>> GetUsuarios(
            [FromQuery] string? busqueda = null,
            [FromQuery] string? rol = null,
            [FromQuery] bool? activo = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var filtro = new UsuarioFiltroDTO
                {
                    Busqueda = busqueda,
                    Rol = rol,
                    Activo = activo,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _usuarioService.GetUsuariosAsync(filtro);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                return StatusCode(500, new { message = "Error al obtener usuarios" });
            }
        }

        /// <summary>
        /// Obtener usuario por ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(UsuarioDetalleDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioDetalleDTO>> GetUsuario(int id)
        {
            try
            {
                var usuario = await _usuarioService.GetUsuarioByIdAsync(id);
                if (usuario == null)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario {Id}", id);
                return StatusCode(500, new { message = "Error al obtener usuario" });
            }
        }

        /// <summary>
        /// Crear nuevo usuario
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(UsuarioDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UsuarioDTO>> CreateUsuario([FromBody] CreateUsuarioDTO dto)
        {
            try
            {
                var adminId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var usuario = await _usuarioService.CreateUsuarioAsync(dto, adminId);
                
                return CreatedAtAction(
                    nameof(GetUsuario), 
                    new { id = usuario.UsuarioID }, 
                    usuario);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                return StatusCode(500, new { message = "Error al crear usuario" });
            }
        }

        /// <summary>
        /// Actualizar usuario existente
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(UsuarioDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioDTO>> UpdateUsuario(int id, [FromBody] UpdateUsuarioDTO dto)
        {
            try
            {
                var adminId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var usuario = await _usuarioService.UpdateUsuarioAsync(id, dto, adminId);
                
                if (usuario == null)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(usuario);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario {Id}", id);
                return StatusCode(500, new { message = "Error al actualizar usuario" });
            }
        }

        /// <summary>
        /// Activar/Desactivar usuario
        /// </summary>
        [HttpPatch("{id}/toggle-status")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ToggleUsuarioStatus(int id)
        {
            try
            {
                var adminId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                await _usuarioService.ToggleUsuarioStatusAsync(id, adminId);
                return Ok(new { message = "Estado del usuario actualizado" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado del usuario {Id}", id);
                return StatusCode(500, new { message = "Error al cambiar estado del usuario" });
            }
        }

        /// <summary>
        /// Resetear contraseña de usuario
        /// </summary>
        [HttpPost("{id}/reset-password")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(ResetPasswordResponseDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResetPasswordResponseDTO>> ResetPassword(int id)
        {
            try
            {
                var adminId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var result = await _usuarioService.ResetPasswordAsync(id, adminId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al resetear contraseña del usuario {Id}", id);
                return StatusCode(500, new { message = "Error al resetear contraseña" });
            }
        }

        /// <summary>
        /// Obtener perfil del usuario actual
        /// </summary>
        [HttpGet("perfil")]
        [ProducesResponseType(typeof(UsuarioPerfilDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<UsuarioPerfilDTO>> GetPerfil()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var perfil = await _usuarioService.GetPerfilUsuarioAsync(userId);
                return Ok(perfil);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener perfil de usuario");
                return StatusCode(500, new { message = "Error al obtener perfil" });
            }
        }

        /// <summary>
        /// Actualizar perfil del usuario actual
        /// </summary>
        [HttpPut("perfil")]
        [ProducesResponseType(typeof(UsuarioPerfilDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<UsuarioPerfilDTO>> UpdatePerfil([FromBody] UpdatePerfilDTO dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var perfil = await _usuarioService.UpdatePerfilUsuarioAsync(userId, dto);
                return Ok(perfil);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar perfil");
                return StatusCode(500, new { message = "Error al actualizar perfil" });
            }
        }

        /// <summary>
        /// Obtener actividad reciente del usuario
        /// </summary>
        [HttpGet("{id}/actividad")]
        [Authorize(Roles = "JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(List<ActividadUsuarioDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ActividadUsuarioDTO>>> GetActividadUsuario(
            int id,
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null,
            [FromQuery] int limit = 50)
        {
            try
            {
                var actividad = await _usuarioService.GetActividadUsuarioAsync(
                    id, 
                    fechaDesde, 
                    fechaHasta, 
                    limit);
                return Ok(actividad);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener actividad del usuario {Id}", id);
                return StatusCode(500, new { message = "Error al obtener actividad" });
            }
        }

        /// <summary>
        /// Obtener roles disponibles
        /// </summary>
        [HttpGet("roles")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(List<RolDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<RolDTO>>> GetRoles()
        {
            try
            {
                var roles = await _usuarioService.GetRolesDisponiblesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener roles");
                return StatusCode(500, new { message = "Error al obtener roles" });
            }
        }

        /// <summary>
        /// Obtener estadísticas de usuarios
        /// </summary>
        [HttpGet("estadisticas")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(EstadisticasUsuariosDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<EstadisticasUsuariosDTO>> GetEstadisticas()
        {
            try
            {
                var stats = await _usuarioService.GetEstadisticasUsuariosAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de usuarios");
                return StatusCode(500, new { message = "Error al obtener estadísticas" });
            }
        }

        /// <summary>
        /// Validar disponibilidad de nombre de usuario
        /// </summary>
        [HttpGet("check-username/{username}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(DisponibilidadDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<DisponibilidadDTO>> CheckUsernameAvailability(string username)
        {
            try
            {
                var disponible = await _usuarioService.IsUsernameAvailableAsync(username);
                return Ok(new DisponibilidadDTO { Disponible = disponible });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar disponibilidad de usuario");
                return StatusCode(500, new { message = "Error al verificar disponibilidad" });
            }
        }

        /// <summary>
        /// Validar disponibilidad de email
        /// </summary>
        [HttpGet("check-email/{email}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(DisponibilidadDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<DisponibilidadDTO>> CheckEmailAvailability(string email)
        {
            try
            {
                var disponible = await _usuarioService.IsEmailAvailableAsync(email);
                return Ok(new DisponibilidadDTO { Disponible = disponible });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar disponibilidad de email");
                return StatusCode(500, new { message = "Error al verificar disponibilidad" });
            }
        }
    }
}
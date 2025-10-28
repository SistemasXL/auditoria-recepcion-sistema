using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuditoriaRecepcion.DTOs.Auditoria;
using AuditoriaRecepcion.DTOs.Common;
using AuditoriaRecepcion.Services.Interfaces;

namespace AuditoriaRecepcion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuditoriasController : ControllerBase
    {
        private readonly IAuditoriaService _auditoriaService;
        private readonly ILogger<AuditoriasController> _logger;

        public AuditoriasController(IAuditoriaService auditoriaService, ILogger<AuditoriasController> logger)
        {
            _auditoriaService = auditoriaService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todas las auditorías con filtros
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<AuditoriaDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResult<AuditoriaDTO>>> GetAuditorias(
            [FromQuery] string? estado = null,
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null,
            [FromQuery] int? usuarioId = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var filtro = new AuditoriaFiltroDTO
                {
                    Estado = estado,
                    FechaDesde = fechaDesde,
                    FechaHasta = fechaHasta,
                    UsuarioId = usuarioId,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _auditoriaService.GetAuditoriasAsync(filtro);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener auditorías");
                return StatusCode(500, new { message = "Error al obtener auditorías" });
            }
        }

        /// <summary>
        /// Obtener auditoría por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AuditoriaDetalleDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuditoriaDetalleDTO>> GetAuditoria(int id)
        {
            try
            {
                var auditoria = await _auditoriaService.GetAuditoriaByIdAsync(id);
                if (auditoria == null)
                    return NotFound(new { message = "Auditoría no encontrada" });

                return Ok(auditoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener auditoría {Id}", id);
                return StatusCode(500, new { message = "Error al obtener auditoría" });
            }
        }

        /// <summary>
        /// Crear nueva auditoría
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Operador,JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(AuditoriaDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuditoriaDTO>> CreateAuditoria([FromBody] CreateAuditoriaDTO dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var auditoria = await _auditoriaService.CreateAuditoriaAsync(dto, userId);
                
                return CreatedAtAction(
                    nameof(GetAuditoria), 
                    new { id = auditoria.AuditoriaID }, 
                    auditoria);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear auditoría");
                return StatusCode(500, new { message = "Error al crear auditoría" });
            }
        }

        /// <summary>
        /// Actualizar auditoría existente
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Operador,JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(AuditoriaDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuditoriaDTO>> UpdateAuditoria(int id, [FromBody] UpdateAuditoriaDTO dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var auditoria = await _auditoriaService.UpdateAuditoriaAsync(id, dto, userId);
                
                if (auditoria == null)
                    return NotFound(new { message = "Auditoría no encontrada" });

                return Ok(auditoria);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar auditoría {Id}", id);
                return StatusCode(500, new { message = "Error al actualizar auditoría" });
            }
        }

        /// <summary>
        /// Cerrar auditoría
        /// </summary>
        [HttpPost("{id}/cerrar")]
        [Authorize(Roles = "Operador,JefeLogistica,Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CerrarAuditoria(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                await _auditoriaService.CerrarAuditoriaAsync(id, userId);
                return Ok(new { message = "Auditoría cerrada correctamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cerrar auditoría {Id}", id);
                return StatusCode(500, new { message = "Error al cerrar auditoría" });
            }
        }

        /// <summary>
        /// Agregar producto a auditoría
        /// </summary>
        [HttpPost("{id}/productos")]
        [Authorize(Roles = "Operador,JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(DetalleAuditoriaDTO), StatusCodes.Status201Created)]
        public async Task<ActionResult<DetalleAuditoriaDTO>> AddProducto(
            int id, 
            [FromBody] AddProductoAuditoriaDTO dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var detalle = await _auditoriaService.AddProductoAsync(id, dto, userId);
                return CreatedAtAction(nameof(GetAuditoria), new { id }, detalle);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar producto a auditoría {Id}", id);
                return StatusCode(500, new { message = "Error al agregar producto" });
            }
        }

        /// <summary>
        /// Actualizar producto en auditoría
        /// </summary>
        [HttpPut("{auditoriaId}/productos/{detalleId}")]
        [Authorize(Roles = "Operador,JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(DetalleAuditoriaDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<DetalleAuditoriaDTO>> UpdateProducto(
            int auditoriaId, 
            int detalleId, 
            [FromBody] UpdateProductoAuditoriaDTO dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var detalle = await _auditoriaService.UpdateProductoAsync(auditoriaId, detalleId, dto, userId);
                return Ok(detalle);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto");
                return StatusCode(500, new { message = "Error al actualizar producto" });
            }
        }

        /// <summary>
        /// Eliminar producto de auditoría
        /// </summary>
        [HttpDelete("{auditoriaId}/productos/{detalleId}")]
        [Authorize(Roles = "Operador,JefeLogistica,Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteProducto(int auditoriaId, int detalleId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                await _auditoriaService.DeleteProductoAsync(auditoriaId, detalleId, userId);
                return Ok(new { message = "Producto eliminado correctamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto");
                return StatusCode(500, new { message = "Error al eliminar producto" });
            }
        }
    }
}
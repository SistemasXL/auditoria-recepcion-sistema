using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuditoriaRecepcion.DTOs.Incidencia;
using AuditoriaRecepcion.DTOs.Common;
using AuditoriaRecepcion.Services.Interfaces;

namespace AuditoriaRecepcion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IncidenciasController : ControllerBase
    {
        private readonly IIncidenciaService _incidenciaService;
        private readonly ILogger<IncidenciasController> _logger;

        public IncidenciasController(IIncidenciaService incidenciaService, ILogger<IncidenciasController> logger)
        {
            _incidenciaService = incidenciaService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todas las incidencias con filtros
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<IncidenciaDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResult<IncidenciaDTO>>> GetIncidencias(
            [FromQuery] string? tipoIncidencia = null,
            [FromQuery] string? estadoResolucion = null,
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null,
            [FromQuery] int? auditoriaId = null,
            [FromQuery] int? proveedorId = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var filtro = new IncidenciaFiltroDTO
                {
                    TipoIncidencia = tipoIncidencia,
                    EstadoResolucion = estadoResolucion,
                    FechaDesde = fechaDesde,
                    FechaHasta = fechaHasta,
                    AuditoriaId = auditoriaId,
                    ProveedorId = proveedorId,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _incidenciaService.GetIncidenciasAsync(filtro);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener incidencias");
                return StatusCode(500, new { message = "Error al obtener incidencias" });
            }
        }

        /// <summary>
        /// Obtener incidencia por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IncidenciaDetalleDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IncidenciaDetalleDTO>> GetIncidencia(int id)
        {
            try
            {
                var incidencia = await _incidenciaService.GetIncidenciaByIdAsync(id);
                if (incidencia == null)
                    return NotFound(new { message = "Incidencia no encontrada" });

                return Ok(incidencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener incidencia {Id}", id);
                return StatusCode(500, new { message = "Error al obtener incidencia" });
            }
        }

        /// <summary>
        /// Crear nueva incidencia
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Operador,JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(IncidenciaDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IncidenciaDTO>> CreateIncidencia([FromBody] CreateIncidenciaDTO dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var incidencia = await _incidenciaService.CreateIncidenciaAsync(dto, userId);
                
                return CreatedAtAction(
                    nameof(GetIncidencia), 
                    new { id = incidencia.IncidenciaID }, 
                    incidencia);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear incidencia");
                return StatusCode(500, new { message = "Error al crear incidencia" });
            }
        }

        /// <summary>
        /// Actualizar incidencia existente
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Operador,JefeLogistica,Comprador,Administrador")]
        [ProducesResponseType(typeof(IncidenciaDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IncidenciaDTO>> UpdateIncidencia(int id, [FromBody] UpdateIncidenciaDTO dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var incidencia = await _incidenciaService.UpdateIncidenciaAsync(id, dto, userId);
                
                if (incidencia == null)
                    return NotFound(new { message = "Incidencia no encontrada" });

                return Ok(incidencia);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar incidencia {Id}", id);
                return StatusCode(500, new { message = "Error al actualizar incidencia" });
            }
        }

        /// <summary>
        /// Cambiar estado de resolución de incidencia
        /// </summary>
        [HttpPatch("{id}/estado")]
        [Authorize(Roles = "JefeLogistica,Comprador,Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoIncidenciaDTO dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                await _incidenciaService.CambiarEstadoAsync(id, dto.EstadoResolucion, dto.Observaciones, userId);
                return Ok(new { message = "Estado actualizado correctamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado de incidencia {Id}", id);
                return StatusCode(500, new { message = "Error al cambiar estado" });
            }
        }

        /// <summary>
        /// Obtener incidencias pendientes por usuario
        /// </summary>
        [HttpGet("pendientes")]
        [Authorize(Roles = "Comprador,JefeLogistica")]
        [ProducesResponseType(typeof(List<IncidenciaDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<IncidenciaDTO>>> GetIncidenciasPendientes()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var incidencias = await _incidenciaService.GetIncidenciasPendientesByUsuarioAsync(userId);
                return Ok(incidencias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener incidencias pendientes");
                return StatusCode(500, new { message = "Error al obtener incidencias pendientes" });
            }
        }

        /// <summary>
        /// Obtener resumen de incidencias
        /// </summary>
        [HttpGet("resumen")]
        [Authorize(Roles = "JefeLogistica,Comprador,Administrador")]
        [ProducesResponseType(typeof(ResumenIncidenciasDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResumenIncidenciasDTO>> GetResumen(
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null)
        {
            try
            {
                var resumen = await _incidenciaService.GetResumenIncidenciasAsync(fechaDesde, fechaHasta);
                return Ok(resumen);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener resumen de incidencias");
                return StatusCode(500, new { message = "Error al obtener resumen" });
            }
        }
    }
}
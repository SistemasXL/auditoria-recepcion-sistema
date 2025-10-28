using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuditoriaRecepcion.DTOs.Proveedor;
using AuditoriaRecepcion.DTOs.Common;
using AuditoriaRecepcion.Services.Interfaces;

namespace AuditoriaRecepcion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProveedoresController : ControllerBase
    {
        private readonly IProveedorService _proveedorService;
        private readonly ILogger<ProveedoresController> _logger;

        public ProveedoresController(IProveedorService proveedorService, ILogger<ProveedoresController> logger)
        {
            _proveedorService = proveedorService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todos los proveedores con filtros
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<ProveedorDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResult<ProveedorDTO>>> GetProveedores(
            [FromQuery] string? busqueda = null,
            [FromQuery] bool? activo = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var filtro = new ProveedorFiltroDTO
                {
                    Busqueda = busqueda,
                    Activo = activo,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _proveedorService.GetProveedoresAsync(filtro);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proveedores");
                return StatusCode(500, new { message = "Error al obtener proveedores" });
            }
        }

        /// <summary>
        /// Obtener proveedor por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProveedorDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProveedorDTO>> GetProveedor(int id)
        {
            try
            {
                var proveedor = await _proveedorService.GetProveedorByIdAsync(id);
                if (proveedor == null)
                    return NotFound(new { message = "Proveedor no encontrado" });

                return Ok(proveedor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proveedor {Id}", id);
                return StatusCode(500, new { message = "Error al obtener proveedor" });
            }
        }

        /// <summary>
        /// Buscar proveedor por CUIT
        /// </summary>
        [HttpGet("cuit/{cuit}")]
        [ProducesResponseType(typeof(ProveedorDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProveedorDTO>> GetProveedorByCuit(string cuit)
        {
            try
            {
                var proveedor = await _proveedorService.GetProveedorByCuitAsync(cuit);
                if (proveedor == null)
                    return NotFound(new { message = "Proveedor no encontrado" });

                return Ok(proveedor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar proveedor por CUIT {CUIT}", cuit);
                return StatusCode(500, new { message = "Error al buscar proveedor" });
            }
        }

        /// <summary>
        /// Crear nuevo proveedor
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Comprador,JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(ProveedorDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProveedorDTO>> CreateProveedor([FromBody] CreateProveedorDTO dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var proveedor = await _proveedorService.CreateProveedorAsync(dto, userId);
                
                return CreatedAtAction(
                    nameof(GetProveedor), 
                    new { id = proveedor.ProveedorID }, 
                    proveedor);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear proveedor");
                return StatusCode(500, new { message = "Error al crear proveedor" });
            }
        }

        /// <summary>
        /// Actualizar proveedor existente
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Comprador,JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(ProveedorDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProveedorDTO>> UpdateProveedor(int id, [FromBody] UpdateProveedorDTO dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var proveedor = await _proveedorService.UpdateProveedorAsync(id, dto, userId);
                
                if (proveedor == null)
                    return NotFound(new { message = "Proveedor no encontrado" });

                return Ok(proveedor);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar proveedor {Id}", id);
                return StatusCode(500, new { message = "Error al actualizar proveedor" });
            }
        }

        /// <summary>
        /// Activar/Desactivar proveedor
        /// </summary>
        [HttpPatch("{id}/toggle-status")]
        [Authorize(Roles = "Comprador,JefeLogistica,Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ToggleProveedorStatus(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                await _proveedorService.ToggleProveedorStatusAsync(id, userId);
                return Ok(new { message = "Estado del proveedor actualizado" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado del proveedor {Id}", id);
                return StatusCode(500, new { message = "Error al cambiar estado del proveedor" });
            }
        }

        /// <summary>
        /// Obtener estadísticas del proveedor
        /// </summary>
        [HttpGet("{id}/estadisticas")]
        [Authorize(Roles = "Comprador,JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(ProveedorEstadisticasDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<ProveedorEstadisticasDTO>> GetEstadisticas(
            int id,
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null)
        {
            try
            {
                var stats = await _proveedorService.GetEstadisticasProveedorAsync(id, fechaDesde, fechaHasta);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas del proveedor {Id}", id);
                return StatusCode(500, new { message = "Error al obtener estadísticas" });
            }
        }
    }
}
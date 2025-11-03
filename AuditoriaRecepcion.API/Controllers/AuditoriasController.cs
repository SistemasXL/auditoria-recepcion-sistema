using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AuditoriaRecepcion.Application.DTOs.Auditoria;
using AuditoriaRecepcion.Application.DTOs.Common;
using AuditoriaRecepcion.Application.Interfaces;

namespace AuditoriaRecepcion.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuditoriasController : ControllerBase
    {
        private readonly IAuditoriaService _auditoriaService;

        public AuditoriasController(IAuditoriaService auditoriaService)
        {
            _auditoriaService = auditoriaService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponseDto<AuditoriaDto>>> GetAuditorias(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? estado = null)
        {
            try
            {
                var result = await _auditoriaService.GetAuditoriasAsync(page, pageSize, search, estado);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener auditorías", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<AuditoriaDto>>> GetAuditoriaById(int id)
        {
            try
            {
                var result = await _auditoriaService.GetAuditoriaByIdAsync(id);
                return Ok(ApiResponseDto<AuditoriaDto>.SuccessResponse(result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseDto<AuditoriaDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<AuditoriaDto>.ErrorResponse("Error interno del servidor"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<AuditoriaDto>>> CreateAuditoria([FromBody] CreateAuditoriaDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var result = await _auditoriaService.CreateAuditoriaAsync(dto, userId);
                return CreatedAtAction(nameof(GetAuditoriaById), new { id = result.Id }, 
                    ApiResponseDto<AuditoriaDto>.SuccessResponse(result, "Auditoría creada exitosamente"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<AuditoriaDto>.ErrorResponse("Error al crear auditoría"));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<AuditoriaDto>>> UpdateAuditoria(int id, [FromBody] UpdateAuditoriaDto dto)
        {
            try
            {
                var result = await _auditoriaService.UpdateAuditoriaAsync(id, dto);
                return Ok(ApiResponseDto<AuditoriaDto>.SuccessResponse(result, "Auditoría actualizada exitosamente"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseDto<AuditoriaDto>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponseDto<AuditoriaDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<AuditoriaDto>.ErrorResponse("Error al actualizar auditoría"));
            }
        }

        [HttpPost("{id}/finalizar")]
        public async Task<ActionResult<ApiResponseDto<AuditoriaDto>>> FinalizarAuditoria(int id)
        {
            try
            {
                var result = await _auditoriaService.FinalizarAuditoriaAsync(id);
                return Ok(ApiResponseDto<AuditoriaDto>.SuccessResponse(result, "Auditoría finalizada exitosamente"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseDto<AuditoriaDto>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponseDto<AuditoriaDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<AuditoriaDto>.ErrorResponse("Error al finalizar auditoría"));
            }
        }

        [HttpPost("{id}/cerrar")]
        public async Task<ActionResult<ApiResponseDto<AuditoriaDto>>> CerrarAuditoria(int id)
        {
            try
            {
                var result = await _auditoriaService.CerrarAuditoriaAsync(id);
                return Ok(ApiResponseDto<AuditoriaDto>.SuccessResponse(result, "Auditoría cerrada exitosamente"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseDto<AuditoriaDto>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponseDto<AuditoriaDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<AuditoriaDto>.ErrorResponse("Error al cerrar auditoría"));
            }
        }

        [HttpGet("{id}/productos")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<ProductoAuditoriaDto>>>> GetProductosAuditoria(int id)
        {
            try
            {
                var result = await _auditoriaService.GetProductosAuditoriaAsync(id);
                return Ok(ApiResponseDto<IEnumerable<ProductoAuditoriaDto>>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<IEnumerable<ProductoAuditoriaDto>>.ErrorResponse("Error al obtener productos"));
            }
        }

        [HttpPost("{id}/productos")]
        public async Task<ActionResult<ApiResponseDto<ProductoAuditoriaDto>>> AgregarProducto(int id, [FromBody] AgregarProductoDto dto)
        {
            try
            {
                var result = await _auditoriaService.AgregarProductoAsync(id, dto);
                return Ok(ApiResponseDto<ProductoAuditoriaDto>.SuccessResponse(result, "Producto agregado exitosamente"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseDto<ProductoAuditoriaDto>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponseDto<ProductoAuditoriaDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<ProductoAuditoriaDto>.ErrorResponse("Error al agregar producto"));
            }
        }
    }
}
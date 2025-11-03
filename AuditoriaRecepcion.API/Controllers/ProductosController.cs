using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuditoriaRecepcion.Application.DTOs.Common;
using AuditoriaRecepcion.Application.DTOs.Producto;
using AuditoriaRecepcion.Application.Interfaces;

namespace AuditoriaRecepcion.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponseDto<ProductoDto>>> GetProductos(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            try
            {
                var result = await _productoService.GetProductosAsync(page, pageSize, search);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener productos", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<ProductoDto>>> GetProductoById(int id)
        {
            try
            {
                var result = await _productoService.GetProductoByIdAsync(id);
                return Ok(ApiResponseDto<ProductoDto>.SuccessResponse(result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseDto<ProductoDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<ProductoDto>.ErrorResponse("Error interno del servidor"));
            }
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<ApiResponseDto<ProductoDto>>> GetProductoByCodigo([FromQuery] string codigo)
        {
            try
            {
                var result = await _productoService.GetProductoByCodigoAsync(codigo);
                return Ok(ApiResponseDto<ProductoDto>.SuccessResponse(result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseDto<ProductoDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<ProductoDto>.ErrorResponse("Error interno del servidor"));
            }
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuditoriaRecepcion.DTOs;
using AuditoriaRecepcion.Services.Interfaces;

namespace AuditoriaRecepcion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(IProductoService productoService, ILogger<ProductosController> logger)
        {
            _productoService = productoService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todos los productos con filtros y paginación
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<ProductoDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResult<ProductoDTO>>> GetProductos(
            [FromQuery] string? busqueda = null,
            [FromQuery] bool? activo = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var filtro = new ProductoFiltroDTO
                {
                    Busqueda = busqueda,
                    Activo = activo,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _productoService.GetProductosAsync(filtro);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos");
                return StatusCode(500, new { message = "Error al obtener productos" });
            }
        }

        /// <summary>
        /// Obtener producto por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductoDTO>> GetProducto(int id)
        {
            try
            {
                var producto = await _productoService.GetProductoByIdAsync(id);
                if (producto == null)
                    return NotFound(new { message = "Producto no encontrado" });

                return Ok(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto {Id}", id);
                return StatusCode(500, new { message = "Error al obtener producto" });
            }
        }

        /// <summary>
        /// Buscar producto por código de barras
        /// </summary>
        [HttpGet("barcode/{codigoBarras}")]
        [ProducesResponseType(typeof(ProductoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductoDTO>> GetProductoByBarcode(string codigoBarras)
        {
            try
            {
                var producto = await _productoService.GetProductoByCodigoBarrasAsync(codigoBarras);
                if (producto == null)
                    return NotFound(new { message = "Producto no encontrado" });

                return Ok(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar producto por código de barras {Codigo}", codigoBarras);
                return StatusCode(500, new { message = "Error al buscar producto" });
            }
        }

        /// <summary>
        /// Buscar producto por SKU
        /// </summary>
        [HttpGet("sku/{sku}")]
        [ProducesResponseType(typeof(ProductoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductoDTO>> GetProductoBySku(string sku)
        {
            try
            {
                var producto = await _productoService.GetProductoBySKUAsync(sku);
                if (producto == null)
                    return NotFound(new { message = "Producto no encontrado" });

                return Ok(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar producto por SKU {SKU}", sku);
                return StatusCode(500, new { message = "Error al buscar producto" });
            }
        }

        /// <summary>
        /// Crear nuevo producto
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(ProductoDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductoDTO>> CreateProducto([FromBody] CreateProductoDTO dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var producto = await _productoService.CreateProductoAsync(dto, userId);
                
                return CreatedAtAction(
                    nameof(GetProducto), 
                    new { id = producto.ProductoID }, 
                    producto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                return StatusCode(500, new { message = "Error al crear producto" });
            }
        }

        /// <summary>
        /// Actualizar producto existente
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(ProductoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductoDTO>> UpdateProducto(int id, [FromBody] UpdateProductoDTO dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var producto = await _productoService.UpdateProductoAsync(id, dto, userId);
                
                if (producto == null)
                    return NotFound(new { message = "Producto no encontrado" });

                return Ok(producto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto {Id}", id);
                return StatusCode(500, new { message = "Error al actualizar producto" });
            }
        }

        /// <summary>
        /// Activar/Desactivar producto
        /// </summary>
        [HttpPatch("{id}/toggle-status")]
        [Authorize(Roles = "JefeLogistica,Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ToggleProductoStatus(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                await _productoService.ToggleProductoStatusAsync(id, userId);
                return Ok(new { message = "Estado del producto actualizado" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado del producto {Id}", id);
                return StatusCode(500, new { message = "Error al cambiar estado del producto" });
            }
        }

        /// <summary>
        /// Eliminar producto (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                await _productoService.DeleteProductoAsync(id, userId);
                return Ok(new { message = "Producto eliminado correctamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto {Id}", id);
                return StatusCode(500, new { message = "Error al eliminar producto" });
            }
        }

        /// <summary>
        /// Importar productos desde Excel
        /// </summary>
        [HttpPost("import")]
        [Authorize(Roles = "JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(ImportResultDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<ImportResultDTO>> ImportProductos(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "Archivo no válido" });

                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var result = await _productoService.ImportProductosAsync(file, userId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al importar productos");
                return StatusCode(500, new { message = "Error al importar productos" });
            }
        }

        /// <summary>
        /// Exportar productos a Excel
        /// </summary>
        [HttpGet("export")]
        [Authorize(Roles = "JefeLogistica,Comprador,Administrador")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportProductos([FromQuery] string? busqueda = null)
        {
            try
            {
                var fileBytes = await _productoService.ExportProductosAsync(busqueda);
                return File(fileBytes, 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    $"Productos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar productos");
                return StatusCode(500, new { message = "Error al exportar productos" });
            }
        }
    }
}
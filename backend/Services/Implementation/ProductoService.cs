using AuditoriaRecepcion.Data;
using AuditoriaRecepcion.DTOs.Producto;
using AuditoriaRecepcion.DTOs.Common;
using AuditoriaRecepcion.Models;
using AuditoriaRecepcion.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using ClosedXML.Excel;
using System.Text;

namespace AuditoriaRecepcion.Services.Implementation
{
    public class ProductoService : IProductoService
    {
        private readonly AuditoriaRecepcionContext _context;
        private readonly ILogger<ProductoService> _logger;

        public ProductoService(
            AuditoriaRecepcionContext context,
            ILogger<ProductoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaginatedResult<ProductoDTO>> GetProductosAsync(ProductoFiltroDTO filtro)
        {
            try
            {
                var query = _context.Productos
                    .AsNoTracking()
                    .AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrEmpty(filtro.Busqueda))
                {
                    query = query.Where(p =>
                        p.SKU.Contains(filtro.Busqueda) ||
                        p.Nombre.Contains(filtro.Busqueda) ||
                        p.CodigoBarras.Contains(filtro.Busqueda) ||
                        p.Descripcion.Contains(filtro.Busqueda));
                }

                if (!string.IsNullOrEmpty(filtro.Categoria))
                    query = query.Where(p => p.Categoria == filtro.Categoria);

                if (filtro.Activo.HasValue)
                    query = query.Where(p => p.Activo == filtro.Activo.Value);

                // Contar total
                var totalItems = await query.CountAsync();

                // Ordenar y paginar
                var items = await query
                    .OrderBy(p => p.Nombre)
                    .Skip((filtro.PageNumber - 1) * filtro.PageSize)
                    .Take(filtro.PageSize)
                    .Select(p => MapToProductoDTO(p))
                    .ToListAsync();

                return new PaginatedResult<ProductoDTO>(items, totalItems, filtro.PageNumber, filtro.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos");
                throw;
            }
        }

        public async Task<ProductoDTO> GetProductoByIdAsync(int id)
        {
            try
            {
                var producto = await _context.Productos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.ProductoID == id);

                return producto != null ? MapToProductoDTO(producto) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto {Id}", id);
                throw;
            }
        }

        public async Task<ProductoDTO> GetProductoByCodigoBarrasAsync(string codigoBarras)
        {
            try
            {
                var producto = await _context.Productos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.CodigoBarras == codigoBarras && p.Activo);

                return producto != null ? MapToProductoDTO(producto) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar producto por código de barras {CodigoBarras}", codigoBarras);
                throw;
            }
        }

        public async Task<ProductoDTO> GetProductoBySKUAsync(string sku)
        {
            try
            {
                var producto = await _context.Productos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.SKU == sku && p.Activo);

                return producto != null ? MapToProductoDTO(producto) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar producto por SKU {SKU}", sku);
                throw;
            }
        }

        public async Task<ProductoDTO> CreateProductoAsync(CreateProductoDTO dto, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validar SKU único
                if (await _context.Productos.AnyAsync(p => p.SKU == dto.SKU))
                    throw new InvalidOperationException($"Ya existe un producto con el SKU: {dto.SKU}");

                // Validar código de barras único si se proporciona
                if (!string.IsNullOrEmpty(dto.CodigoBarras) &&
                    await _context.Productos.AnyAsync(p => p.CodigoBarras == dto.CodigoBarras))
                    throw new InvalidOperationException($"Ya existe un producto con el código de barras: {dto.CodigoBarras}");

                var producto = new Producto
                {
                    SKU = dto.SKU,
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    CodigoBarras = dto.CodigoBarras,
                    Categoria = dto.Categoria,
                    UnidadMedida = dto.UnidadMedida,
                    PesoUnitario = dto.PesoUnitario,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();

                // Registrar auditoría
                await RegistrarAuditoriaAccionAsync(userId, "Crear", "Producto", producto.ProductoID);

                await transaction.CommitAsync();

                _logger.LogInformation("Producto creado: {SKU} por usuario {UserId}", dto.SKU, userId);

                return MapToProductoDTO(producto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al crear producto");
                throw;
            }
        }

        public async Task<ProductoDTO> UpdateProductoAsync(int id, UpdateProductoDTO dto, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null)
                    return null;

                // Validar código de barras único si cambió
                if (!string.IsNullOrEmpty(dto.CodigoBarras) &&
                    dto.CodigoBarras != producto.CodigoBarras &&
                    await _context.Productos.AnyAsync(p => p.CodigoBarras == dto.CodigoBarras && p.ProductoID != id))
                    throw new InvalidOperationException($"Ya existe un producto con el código de barras: {dto.CodigoBarras}");

                producto.Nombre = dto.Nombre;
                producto.Descripcion = dto.Descripcion;
                producto.CodigoBarras = dto.CodigoBarras;
                producto.Categoria = dto.Categoria;
                producto.UnidadMedida = dto.UnidadMedida;
                producto.PesoUnitario = dto.PesoUnitario;

                await _context.SaveChangesAsync();

                // Registrar auditoría
                await RegistrarAuditoriaAccionAsync(userId, "Actualizar", "Producto", id);

                await transaction.CommitAsync();

                _logger.LogInformation("Producto actualizado: {Id} por usuario {UserId}", id, userId);

                return MapToProductoDTO(producto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al actualizar producto {Id}", id);
                throw;
            }
        }

        public async Task ToggleProductoStatusAsync(int id, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null)
                    throw new InvalidOperationException("Producto no encontrado");

                producto.Activo = !producto.Activo;
                await _context.SaveChangesAsync();

                // Registrar auditoría
                await RegistrarAuditoriaAccionAsync(userId, "CambiarEstado", "Producto", id);

                await transaction.CommitAsync();

                _logger.LogInformation("Estado de producto cambiado: {Id} a {Estado} por usuario {UserId}",
                    id, producto.Activo ? "Activo" : "Inactivo", userId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al cambiar estado del producto {Id}", id);
                throw;
            }
        }

        public async Task DeleteProductoAsync(int id, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null)
                    throw new InvalidOperationException("Producto no encontrado");

                // Verificar si el producto está siendo usado en auditorías
                var enUso = await _context.DetallesAuditoria.AnyAsync(d => d.ProductoID == id);
                if (enUso)
                    throw new InvalidOperationException("No se puede eliminar el producto porque está siendo usado en auditorías");

                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();

                // Registrar auditoría
                await RegistrarAuditoriaAccionAsync(userId, "Eliminar", "Producto", id);

                await transaction.CommitAsync();

                _logger.LogInformation("Producto eliminado: {Id} por usuario {UserId}", id, userId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al eliminar producto {Id}", id);
                throw;
            }
        }

        public async Task<ImportResultDTO> ImportProductosAsync(IFormFile file, int userId)
        {
            var result = new ImportResultDTO();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (file == null || file.Length == 0)
                    throw new InvalidOperationException("Archivo vacío o inválido");

                if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xls"))
                    throw new InvalidOperationException("Solo se permiten archivos Excel (.xlsx, .xls)");

                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Saltar encabezado

                foreach (var row in rows)
                {
                    result.TotalRegistros++;

                    try
                    {
                        var sku = row.Cell(1).GetString();
                        var nombre = row.Cell(2).GetString();
                        var descripcion = row.Cell(3).GetString();
                        var codigoBarras = row.Cell(4).GetString();
                        var categoria = row.Cell(5).GetString();
                        var unidadMedida = row.Cell(6).GetString();
                        var pesoUnitario = row.Cell(7).TryGetValue(out decimal peso) ? peso : (decimal?)null;

                        // Validar SKU único
                        if (await _context.Productos.AnyAsync(p => p.SKU == sku))
                        {
                            result.Fallidos++;
                            result.Errores.Add($"Fila {result.TotalRegistros}: SKU duplicado - {sku}");
                            continue;
                        }

                        var producto = new Producto
                        {
                            SKU = sku,
                            Nombre = nombre,
                            Descripcion = descripcion,
                            CodigoBarras = codigoBarras,
                            Categoria = categoria,
                            UnidadMedida = unidadMedida,
                            PesoUnitario = pesoUnitario,
                            Activo = true,
                            FechaCreacion = DateTime.Now
                        };

                        _context.Productos.Add(producto);
                        result.Exitosos++;
                        result.ProductosImportados.Add(MapToProductoDTO(producto));
                    }
                    catch (Exception ex)
                    {
                        result.Fallidos++;
                        result.Errores.Add($"Fila {result.TotalRegistros}: {ex.Message}");
                    }
                }

                await _context.SaveChangesAsync();

                // Registrar auditoría
                await RegistrarAuditoriaAccionAsync(userId, "ImportarProductos", "Producto", 0);

                await transaction.CommitAsync();

                _logger.LogInformation("Importación de productos completada: {Exitosos} exitosos, {Fallidos} fallidos",
                    result.Exitosos, result.Fallidos);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error en importación de productos");
                throw;
            }

            return result;
        }

        public async Task<byte[]> ExportProductosAsync(string busqueda = null)
        {
            try
            {
                var query = _context.Productos.AsNoTracking().AsQueryable();

                if (!string.IsNullOrEmpty(busqueda))
                {
                    query = query.Where(p =>
                        p.SKU.Contains(busqueda) ||
                        p.Nombre.Contains(busqueda) ||
                        p.CodigoBarras.Contains(busqueda));
                }

                var productos = await query.OrderBy(p => p.Nombre).ToListAsync();

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Productos");

                // Encabezados
                worksheet.Cell(1, 1).Value = "SKU";
                worksheet.Cell(1, 2).Value = "Nombre";
                worksheet.Cell(1, 3).Value = "Descripción";
                worksheet.Cell(1, 4).Value = "Código de Barras";
                worksheet.Cell(1, 5).Value = "Categoría";
                worksheet.Cell(1, 6).Value = "Unidad de Medida";
                worksheet.Cell(1, 7).Value = "Peso Unitario";
                worksheet.Cell(1, 8).Value = "Activo";
                worksheet.Cell(1, 9).Value = "Fecha Creación";

                // Formato de encabezados
                var headerRange = worksheet.Range(1, 1, 1, 9);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Datos
                int row = 2;
                foreach (var producto in productos)
                {
                    worksheet.Cell(row, 1).Value = producto.SKU;
                    worksheet.Cell(row, 2).Value = producto.Nombre;
                    worksheet.Cell(row, 3).Value = producto.Descripcion;
                    worksheet.Cell(row, 4).Value = producto.CodigoBarras;
                    worksheet.Cell(row, 5).Value = producto.Categoria;
                    worksheet.Cell(row, 6).Value = producto.UnidadMedida;
                    worksheet.Cell(row, 7).Value = producto.PesoUnitario;
                    worksheet.Cell(row, 8).Value = producto.Activo ? "Sí" : "No";
                    worksheet.Cell(row, 9).Value = producto.FechaCreacion.ToString("dd/MM/yyyy");
                    row++;
                }

                // Ajustar ancho de columnas
                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar productos");
                throw;
            }
        }

        // Métodos privados auxiliares
        private async Task RegistrarAuditoriaAccionAsync(int userId, string accion, string tabla, int registroId)
        {
            var auditoria = new AuditoriaAccion
            {
                UsuarioID = userId,
                TipoAccion = accion,
                TablaAfectada = tabla,
                RegistroID = registroId,
                FechaHora = DateTime.Now
            };

            _context.AuditoriasAcciones.Add(auditoria);
            await _context.SaveChangesAsync();
        }

        private ProductoDTO MapToProductoDTO(Producto producto)
        {
            return new ProductoDTO
            {
                ProductoID = producto.ProductoID,
                SKU = producto.SKU,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                CodigoBarras = producto.CodigoBarras,
                Categoria = producto.Categoria,
                UnidadMedida = producto.UnidadMedida,
                PesoUnitario = producto.PesoUnitario,
                Activo = producto.Activo,
                FechaCreacion = producto.FechaCreacion
            };
        }
    }
}
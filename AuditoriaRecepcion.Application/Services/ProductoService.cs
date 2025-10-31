using Microsoft.EntityFrameworkCore;
using AuditoriaRecepcion.Application.DTOs.Common;
using AuditoriaRecepcion.Application.DTOs.Producto;
using AuditoriaRecepcion.Application.Interfaces;
using AuditoriaRecepcion.Infrastructure.Data;

namespace AuditoriaRecepcion.Application.Services
{
    public class ProductoService : IProductoService
    {
        private readonly ApplicationDbContext _context;

        public ProductoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResponseDto<ProductoDto>> GetProductosAsync(
            int page, 
            int pageSize, 
            string? search = null)
        {
            var query = _context.Productos.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => 
                    p.Sku.Contains(search) ||
                    p.Nombre.Contains(search) ||
                    p.CodigoBarras.Contains(search));
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var productos = await query
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductoDto
                {
                    Id = p.Id,
                    Sku = p.Sku,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    CodigoBarras = p.CodigoBarras,
                    Categoria = p.Categoria,
                    UnidadMedida = p.UnidadMedida,
                    PrecioUnitario = p.PrecioUnitario,
                    Activo = p.Activo
                })
                .ToListAsync();

            return new PaginatedResponseDto<ProductoDto>
            {
                Items = productos,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = page > 1,
                HasNextPage = page < totalPages
            };
        }

        public async Task<ProductoDto> GetProductoByIdAsync(int id)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
                throw new KeyNotFoundException($"Producto con ID {id} no encontrado");

            return new ProductoDto
            {
                Id = producto.Id,
                Sku = producto.Sku,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                CodigoBarras = producto.CodigoBarras,
                Categoria = producto.Categoria,
                UnidadMedida = producto.UnidadMedida,
                PrecioUnitario = producto.PrecioUnitario,
                Activo = producto.Activo
            };
        }

        public async Task<ProductoDto> GetProductoByCodigoAsync(string codigo)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.CodigoBarras == codigo && p.Activo);

            if (producto == null)
                throw new KeyNotFoundException($"Producto con código {codigo} no encontrado");

            return new ProductoDto
            {
                Id = producto.Id,
                Sku = producto.Sku,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                CodigoBarras = producto.CodigoBarras,
                Categoria = producto.Categoria,
                UnidadMedida = producto.UnidadMedida,
                PrecioUnitario = producto.PrecioUnitario,
                Activo = producto.Activo
            };
        }
    }
}
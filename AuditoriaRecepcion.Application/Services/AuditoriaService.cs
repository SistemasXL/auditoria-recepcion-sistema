using Microsoft.EntityFrameworkCore;
using AuditoriaRecepcion.Application.DTOs.Auditoria;
using AuditoriaRecepcion.Application.DTOs.Common;
using AuditoriaRecepcion.Application.Interfaces;
using AuditoriaRecepcion.Core.Entities;
using AuditoriaRecepcion.Core.Enums;
using AuditoriaRecepcion.Core.Interfaces;
using AuditoriaRecepcion.Infrastructure.Data;

namespace AuditoriaRecepcion.Application.Services
{
    public class AuditoriaService : IAuditoriaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public AuditoriaService(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<PaginatedResponseDto<AuditoriaDto>> GetAuditoriasAsync(
            int page, 
            int pageSize, 
            string? search = null, 
            string? estado = null)
        {
            var query = _context.Auditorias
                .Include(a => a.Proveedor)
                .Include(a => a.UsuarioCreador)
                .Include(a => a.Productos)
                .Include(a => a.Incidencias)
                .AsQueryable();

            // Filtros
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => 
                    a.NumeroAuditoria.Contains(search) ||
                    a.OrdenCompra.Contains(search) ||
                    a.Proveedor.Nombre.Contains(search));
            }

            if (!string.IsNullOrEmpty(estado) && Enum.TryParse<EstadoAuditoria>(estado, out var estadoEnum))
            {
                query = query.Where(a => a.Estado == estadoEnum);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var auditorias = await query
                .OrderByDescending(a => a.FechaCreacion)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AuditoriaDto
                {
                    Id = a.Id,
                    NumeroAuditoria = a.NumeroAuditoria,
                    Fecha = a.Fecha,
                    Hora = a.Hora.ToString(@"hh\:mm"),
                    Proveedor = new ProveedorSimpleDto
                    {
                        Id = a.Proveedor.Id,
                        Codigo = a.Proveedor.Codigo,
                        Nombre = a.Proveedor.Nombre
                    },
                    OrdenCompra = a.OrdenCompra,
                    Estado = a.Estado.ToString(),
                    Observaciones = a.Observaciones,
                    TotalProductos = a.Productos.Count,
                    TieneIncidencias = a.Incidencias.Any(),
                    UsuarioCreador = a.UsuarioCreador.NombreCompleto,
                    FechaCreacion = a.FechaCreacion,
                    FechaFinalizacion = a.FechaFinalizacion,
                    FechaCierre = a.FechaCierre
                })
                .ToListAsync();

            return new PaginatedResponseDto<AuditoriaDto>
            {
                Items = auditorias,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = page > 1,
                HasNextPage = page < totalPages
            };
        }

        public async Task<AuditoriaDto> GetAuditoriaByIdAsync(int id)
        {
            var auditoria = await _context.Auditorias
                .Include(a => a.Proveedor)
                .Include(a => a.UsuarioCreador)
                .Include(a => a.Productos)
                .Include(a => a.Incidencias)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (auditoria == null)
                throw new KeyNotFoundException($"Auditoría con ID {id} no encontrada");

            return new AuditoriaDto
            {
                Id = auditoria.Id,
                NumeroAuditoria = auditoria.NumeroAuditoria,
                Fecha = auditoria.Fecha,
                Hora = auditoria.Hora.ToString(@"hh\:mm"),
                Proveedor = new ProveedorSimpleDto
                {
                    Id = auditoria.Proveedor.Id,
                    Codigo = auditoria.Proveedor.Codigo,
                    Nombre = auditoria.Proveedor.Nombre
                },
                OrdenCompra = auditoria.OrdenCompra,
                Estado = auditoria.Estado.ToString(),
                Observaciones = auditoria.Observaciones,
                TotalProductos = auditoria.Productos.Count,
                TieneIncidencias = auditoria.Incidencias.Any(),
                UsuarioCreador = auditoria.UsuarioCreador.NombreCompleto,
                FechaCreacion = auditoria.FechaCreacion,
                FechaFinalizacion = auditoria.FechaFinalizacion,
                FechaCierre = auditoria.FechaCierre
            };
        }

        public async Task<AuditoriaDto> CreateAuditoriaAsync(CreateAuditoriaDto dto, int usuarioId)
        {
            // Generar número de auditoría
            var ultimaAuditoria = await _context.Auditorias
                .OrderByDescending(a => a.Id)
                .FirstOrDefaultAsync();

            var numeroSecuencia = ultimaAuditoria != null ? ultimaAuditoria.Id + 1 : 1;
            var numeroAuditoria = $"AUD-{numeroSecuencia:D6}";

            var auditoria = new Auditoria
            {
                NumeroAuditoria = numeroAuditoria,
                ProveedorId = dto.ProveedorId,
                OrdenCompra = dto.OrdenCompra,
                Fecha = dto.Fecha,
                Hora = dto.Hora,
                Observaciones = dto.Observaciones,
                Estado = EstadoAuditoria.EnProceso,
                UsuarioCreadorId = usuarioId,
                FechaCreacion = DateTime.UtcNow
            };

            await _unitOfWork.Auditorias.AddAsync(auditoria);
            await _unitOfWork.SaveChangesAsync();

            return await GetAuditoriaByIdAsync(auditoria.Id);
        }

        public async Task<AuditoriaDto> UpdateAuditoriaAsync(int id, UpdateAuditoriaDto dto)
        {
            var auditoria = await _unitOfWork.Auditorias.GetByIdAsync(id);
            if (auditoria == null)
                throw new KeyNotFoundException($"Auditoría con ID {id} no encontrada");

            if (auditoria.Estado != EstadoAuditoria.Borrador && auditoria.Estado != EstadoAuditoria.EnProceso)
                throw new InvalidOperationException("Solo se pueden editar auditorías en estado Borrador o En Proceso");

            if (dto.ProveedorId.HasValue)
                auditoria.ProveedorId = dto.ProveedorId.Value;
            if (dto.OrdenCompra != null)
                auditoria.OrdenCompra = dto.OrdenCompra;
            if (dto.Fecha.HasValue)
                auditoria.Fecha = dto.Fecha.Value;
            if (dto.Hora.HasValue)
                auditoria.Hora = dto.Hora.Value;
            if (dto.Observaciones != null)
                auditoria.Observaciones = dto.Observaciones;

            auditoria.FechaModificacion = DateTime.UtcNow;

            await _unitOfWork.Auditorias.UpdateAsync(auditoria);
            await _unitOfWork.SaveChangesAsync();

            return await GetAuditoriaByIdAsync(id);
        }

        public async Task<AuditoriaDto> FinalizarAuditoriaAsync(int id)
        {
            var auditoria = await _unitOfWork.Auditorias.GetByIdAsync(id);
            if (auditoria == null)
                throw new KeyNotFoundException($"Auditoría con ID {id} no encontrada");

            if (auditoria.Estado == EstadoAuditoria.Finalizada)
                throw new InvalidOperationException("La auditoría ya está finalizada");

            var productos = await _context.ProductosAuditoria
                .Where(pa => pa.AuditoriaId == id)
                .ToListAsync();

            if (!productos.Any())
                throw new InvalidOperationException("No se puede finalizar una auditoría sin productos");

            auditoria.Estado = EstadoAuditoria.Finalizada;
            auditoria.FechaFinalizacion = DateTime.UtcNow;
            auditoria.FechaModificacion = DateTime.UtcNow;

            await _unitOfWork.Auditorias.UpdateAsync(auditoria);
            await _unitOfWork.SaveChangesAsync();

            return await GetAuditoriaByIdAsync(id);
        }

        public async Task<AuditoriaDto> CerrarAuditoriaAsync(int id)
        {
            var auditoria = await _unitOfWork.Auditorias.GetByIdAsync(id);
            if (auditoria == null)
                throw new KeyNotFoundException($"Auditoría con ID {id} no encontrada");

            if (auditoria.Estado != EstadoAuditoria.Finalizada)
                throw new InvalidOperationException("Solo se pueden cerrar auditorías finalizadas");

            auditoria.Estado = EstadoAuditoria.Cerrada;
            auditoria.FechaCierre = DateTime.UtcNow;
            auditoria.FechaModificacion = DateTime.UtcNow;

            await _unitOfWork.Auditorias.UpdateAsync(auditoria);
            await _unitOfWork.SaveChangesAsync();

            return await GetAuditoriaByIdAsync(id);
        }

        public async Task<IEnumerable<ProductoAuditoriaDto>> GetProductosAuditoriaAsync(int auditoriaId)
        {
            var productos = await _context.ProductosAuditoria
                .Include(pa => pa.Producto)
                .Where(pa => pa.AuditoriaId == auditoriaId)
                .Select(pa => new ProductoAuditoriaDto
                {
                    Id = pa.Id,
                    Producto = new ProductoSimpleDto
                    {
                        Id = pa.Producto.Id,
                        Sku = pa.Producto.Sku,
                        Nombre = pa.Producto.Nombre,
                        CodigoBarras = pa.Producto.CodigoBarras,
                        UnidadMedida = pa.Producto.UnidadMedida
                    },
                    CantidadEsperada = pa.CantidadEsperada,
                    CantidadRecibida = pa.CantidadRecibida,
                    CantidadDiferencia = pa.CantidadDiferencia,
                    Observaciones = pa.Observaciones,
                    FechaRegistro = pa.FechaRegistro
                })
                .ToListAsync();

            return productos;
        }

        public async Task<ProductoAuditoriaDto> AgregarProductoAsync(int auditoriaId, AgregarProductoDto dto)
        {
            var auditoria = await _unitOfWork.Auditorias.GetByIdAsync(auditoriaId);
            if (auditoria == null)
                throw new KeyNotFoundException($"Auditoría con ID {auditoriaId} no encontrada");

            if (auditoria.Estado == EstadoAuditoria.Cerrada || auditoria.Estado == EstadoAuditoria.Cancelada)
                throw new InvalidOperationException("No se pueden agregar productos a una auditoría cerrada o cancelada");

            var producto = await _unitOfWork.Productos.GetByIdAsync(dto.ProductoId);
            if (producto == null)
                throw new KeyNotFoundException($"Producto con ID {dto.ProductoId} no encontrado");

            var productoAuditoria = new ProductoAuditoria
            {
                AuditoriaId = auditoriaId,
                ProductoId = dto.ProductoId,
                CantidadEsperada = dto.CantidadEsperada,
                CantidadRecibida = dto.CantidadRecibida,
                CantidadDiferencia = dto.CantidadRecibida - dto.CantidadEsperada,
                Observaciones = dto.Observaciones,
                FechaRegistro = DateTime.UtcNow
            };

            await _unitOfWork.ProductosAuditoria.AddAsync(productoAuditoria);
            await _unitOfWork.SaveChangesAsync();

            var result = await _context.ProductosAuditoria
                .Include(pa => pa.Producto)
                .FirstOrDefaultAsync(pa => pa.Id == productoAuditoria.Id);

            return new ProductoAuditoriaDto
            {
                Id = result!.Id,
                Producto = new ProductoSimpleDto
                {
                    Id = result.Producto.Id,
                    Sku = result.Producto.Sku,
                    Nombre = result.Producto.Nombre,
                    CodigoBarras = result.Producto.CodigoBarras,
                    UnidadMedida = result.Producto.UnidadMedida
                },
                CantidadEsperada = result.CantidadEsperada,
                CantidadRecibida = result.CantidadRecibida,
                CantidadDiferencia = result.CantidadDiferencia,
                Observaciones = result.Observaciones,
                FechaRegistro = result.FechaRegistro
            };
        }
    }
}
using AuditoriaRecepcion.Data;
using AuditoriaRecepcion.DTOs.Auditoria;
using AuditoriaRecepcion.DTOs.Common;
using AuditoriaRecepcion.Models;
using AuditoriaRecepcion.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuditoriaRecepcion.Services.Implementation
{
    public class AuditoriaService : IAuditoriaService
    {
        private readonly AuditoriaRecepcionContext _context;
        private readonly INotificationService _notificationService;
        private readonly ILogger<AuditoriaService> _logger;

        public AuditoriaService(
            AuditoriaRecepcionContext context,
            INotificationService notificationService,
            ILogger<AuditoriaService> logger)
        {
            _context = context;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<PaginatedResult<AuditoriaDTO>> GetAuditoriasAsync(AuditoriaFiltroDTO filtro)
        {
            try
            {
                var query = _context.AuditoriasRecepcion
                    .Include(a => a.Proveedor)
                    .Include(a => a.UsuarioAuditor)
                    .Include(a => a.Detalles)
                    .Include(a => a.Incidencias)
                    .AsNoTracking()
                    .AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrEmpty(filtro.Estado))
                    query = query.Where(a => a.EstadoAuditoria == filtro.Estado);

                if (filtro.FechaDesde.HasValue)
                    query = query.Where(a => a.FechaInicio >= filtro.FechaDesde.Value);

                if (filtro.FechaHasta.HasValue)
                    query = query.Where(a => a.FechaInicio <= filtro.FechaHasta.Value);

                if (filtro.UsuarioId.HasValue)
                    query = query.Where(a => a.UsuarioCreacionID == filtro.UsuarioId.Value);

                if (filtro.ProveedorId.HasValue)
                    query = query.Where(a => a.ProveedorID == filtro.ProveedorId.Value);

                if (!string.IsNullOrEmpty(filtro.NumeroOrdenCompra))
                    query = query.Where(a => a.NumeroOrdenCompra.Contains(filtro.NumeroOrdenCompra));

                // Contar total
                var totalItems = await query.CountAsync();

                // Ordenar y paginar
                var items = await query
                    .OrderByDescending(a => a.FechaCreacion)
                    .Skip((filtro.PageNumber - 1) * filtro.PageSize)
                    .Take(filtro.PageSize)
                    .Select(a => new AuditoriaDTO
                    {
                        AuditoriaID = a.AuditoriaID,
                        NumeroAuditoria = a.NumeroAuditoria,
                        FechaRecepcion = a.FechaInicio,
                        ProveedorID = a.ProveedorID,
                        ProveedorNombre = a.Proveedor.RazonSocial,
                        NumeroOrdenCompra = a.NumeroOrdenCompra,
                        NumeroRemito = a.NumeroRemito,
                        Estado = a.EstadoAuditoria,
                        CantidadProductos = a.Detalles.Count,
                        CantidadIncidencias = a.Incidencias.Count,
                        UsuarioCreacionID = a.UsuarioCreacionID,
                        UsuarioCreacionNombre = a.UsuarioAuditor.NombreCompleto,
                        FechaCreacion = a.FechaCreacion,
                        FechaCierre = a.FechaFinalizacion,
                        Observaciones = a.Observaciones
                    })
                    .ToListAsync();

                return new PaginatedResult<AuditoriaDTO>(items, totalItems, filtro.PageNumber, filtro.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener auditorías");
                throw;
            }
        }

        public async Task<AuditoriaDetalleDTO> GetAuditoriaByIdAsync(int id)
        {
            try
            {
                var auditoria = await _context.AuditoriasRecepcion
                    .Include(a => a.Proveedor)
                    .Include(a => a.UsuarioAuditor)
                    .Include(a => a.UsuarioModificacion)
                    .Include(a => a.Detalles)
                        .ThenInclude(d => d.Producto)
                    .Include(a => a.Incidencias)
                        .ThenInclude(i => i.UsuarioReporto)
                    .Include(a => a.Evidencias)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.AuditoriaID == id);

                if (auditoria == null)
                    return null;

                return MapToAuditoriaDetalleDTO(auditoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener auditoría {Id}", id);
                throw;
            }
        }

        public async Task<AuditoriaDTO> CreateAuditoriaAsync(CreateAuditoriaDTO dto, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validar proveedor
                var proveedor = await _context.Proveedores.FindAsync(dto.ProveedorID);
                if (proveedor == null)
                    throw new InvalidOperationException("Proveedor no encontrado");

                // Generar número de auditoría
                var numeroAuditoria = await GenerarNumeroAuditoriaAsync();

                var auditoria = new Auditoria
                {
                    NumeroAuditoria = numeroAuditoria,
                    FechaRecepcion = dto.FechaInicio,
                    ProveedorID = dto.ProveedorID,
                    NumeroOrdenCompra = dto.NumeroOrdenCompra,
                    NumeroRemito = dto.NumeroRemito,
                    Estado = "Abierta",
                    Observaciones = dto.Observaciones,
                    UsuarioCreacionID = userId,
                    FechaCreacion = DateTime.Now
                };

                _context.AuditoriasRecepcion.Add(auditoria);
                await _context.SaveChangesAsync();

                // Registrar auditoría de acción
                await RegistrarAuditoriaAccionAsync(userId, "Crear", "Auditoria", auditoria.AuditoriaID);

                await transaction.CommitAsync();

                _logger.LogInformation("Auditoría creada: {NumeroAuditoria} por usuario {UserId}", numeroAuditoria, userId);

                return MapToAuditoriaDTO(auditoria, proveedor);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al crear auditoría");
                throw;
            }
        }

        public async Task<AuditoriaDTO> UpdateAuditoriaAsync(int id, UpdateAuditoriaDTO dto, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var auditoria = await _context.AuditoriasRecepcion
                    .Include(a => a.Proveedor)
                    .FirstOrDefaultAsync(a => a.AuditoriaID == id);

                if (auditoria == null)
                    return null;

                if (auditoria.EstadoAuditoria == "Cerrada")
                    throw new InvalidOperationException("No se puede modificar una auditoría cerrada");

                // Actualizar datos
                auditoria.FechaInicio = dto.FechaInicio;
                auditoria.ProveedorID = dto.ProveedorID;
                auditoria.NumeroOrdenCompra = dto.NumeroOrdenCompra;
                auditoria.NumeroRemito = dto.NumeroRemito;
                auditoria.Observaciones = dto.Observaciones;
                auditoria.UsuarioModificacionID = userId;
                auditoria.FechaModificacion = DateTime.Now;

                await _context.SaveChangesAsync();

                // Registrar auditoría de acción
                await RegistrarAuditoriaAccionAsync(userId, "Actualizar", "Auditoria", id);

                await transaction.CommitAsync();

                _logger.LogInformation("Auditoría actualizada: {Id} por usuario {UserId}", id, userId);

                return MapToAuditoriaDTO(auditoria, auditoria.Proveedor);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al actualizar auditoría {Id}", id);
                throw;
            }
        }

        public async Task CerrarAuditoriaAsync(int id, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var auditoria = await _context.AuditoriasRecepcion
                    .Include(a => a.Incidencias)
                    .FirstOrDefaultAsync(a => a.AuditoriaID == id);

                if (auditoria == null)
                    throw new InvalidOperationException("Auditoría no encontrada");

                if (auditoria.EstadoAuditoria == "Cerrada")
                    throw new InvalidOperationException("La auditoría ya está cerrada");

                // Verificar incidencias pendientes
                var incidenciasPendientes = auditoria.Incidencias
                    .Count(i => i.EstadoIncidencia == "Pendiente" || i.EstadoIncidencia == "EnProceso");

                if (incidenciasPendientes > 0)
                    throw new InvalidOperationException($"No se puede cerrar la auditoría. Hay {incidenciasPendientes} incidencia(s) pendiente(s)");

                auditoria.EstadoAuditoria = "Cerrada";
                auditoria.FechaFinalizacion = DateTime.Now;
                auditoria.UsuarioModificacionID = userId;
                auditoria.FechaModificacion = DateTime.Now;

                await _context.SaveChangesAsync();

                // Registrar auditoría de acción
                await RegistrarAuditoriaAccionAsync(userId, "Cerrar", "Auditoria", id);

                await transaction.CommitAsync();

                _logger.LogInformation("Auditoría cerrada: {Id} por usuario {UserId}", id, userId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al cerrar auditoría {Id}", id);
                throw;
            }
        }

        public async Task DeleteAuditoriaAsync(int id, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var auditoria = await _context.AuditoriasRecepcion.FindAsync(id);
                if (auditoria == null)
                    throw new InvalidOperationException("Auditoría no encontrada");

                if (auditoria.EstadoAuditoria == "Cerrada")
                    throw new InvalidOperationException("No se puede eliminar una auditoría cerrada");

                _context.AuditoriasRecepcion.Remove(auditoria);
                await _context.SaveChangesAsync();

                // Registrar auditoría de acción
                await RegistrarAuditoriaAccionAsync(userId, "Eliminar", "Auditoria", id);

                await transaction.CommitAsync();

                _logger.LogInformation("Auditoría eliminada: {Id} por usuario {UserId}", id, userId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al eliminar auditoría {Id}", id);
                throw;
            }
        }

        public async Task<DetalleAuditoriaDTO> AddProductoAsync(int auditoriaId, AddProductoAuditoriaDTO dto, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var auditoria = await _context.AuditoriasRecepcion.FindAsync(auditoriaId);
                if (auditoria == null)
                    throw new InvalidOperationException("Auditoría no encontrada");

                if (auditoria.EstadoAuditoria == "Cerrada")
                    throw new InvalidOperationException("No se pueden agregar productos a una auditoría cerrada");

                var producto = await _context.Productos.FindAsync(dto.ProductoID);
                if (producto == null)
                    throw new InvalidOperationException("Producto no encontrado");

                var detalle = new AuditoriaDetalle
                {
                    AuditoriaID = auditoriaId,
                    ProductoID = dto.ProductoID,
                    CantidadEsperada = dto.CantidadEsperada,
                    CantidadRecibida = dto.CantidadRecibida,
                    EstadoItem = dto.EstadoProducto,
                    Observaciones = dto.Observaciones,
                    FechaRegistro = DateTime.Now
                };

                _context.AuditoriaDetalle.Add(detalle);

                // Si hay diferencias o daños, crear incidencia automáticamente
                if (dto.CantidadEsperada != dto.CantidadRecibida || dto.EstadoProducto != "Bueno")
                {
                    var tipoIncidencia = dto.EstadoProducto == "Dañado" ? "Dañado" :
                                        dto.CantidadEsperada > dto.CantidadRecibida ? "Faltante" : "Excedente";

                    var incidencia = new Incidencia
                    {
                        AuditoriaID = auditoriaId,
                        ProductoID = dto.ProductoID,
                        TipoIncidencia = tipoIncidencia,
                        Descripcion = $"Diferencia detectada: Esperado {dto.CantidadEsperada}, Recibido {dto.CantidadRecibida}. Estado: {dto.EstadoProducto}",
                        EstadoResolucion = "Pendiente",
                        Prioridad = "Media",
                        UsuarioReportoID = userId,
                        FechaDeteccion = DateTime.Now
                    };

                    _context.Incidencias.Add(incidencia);

                    // Notificar al responsable de compras
                    await _notificationService.NotificarNuevaIncidenciaAsync(incidencia.IncidenciaID);
                }

                await _context.SaveChangesAsync();

                // Registrar auditoría de acción
                await RegistrarAuditoriaAccionAsync(userId, "AgregarProducto", "DetalleAuditoria", detalle.DetalleAuditoriaID);

                await transaction.CommitAsync();

                _logger.LogInformation("Producto agregado a auditoría: {AuditoriaId} por usuario {UserId}", auditoriaId, userId);

                return MapToDetalleAuditoriaDTO(detalle, producto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al agregar producto a auditoría {AuditoriaId}", auditoriaId);
                throw;
            }
        }

        public async Task<DetalleAuditoriaDTO> UpdateProductoAsync(int auditoriaId, int detalleId, UpdateProductoAuditoriaDTO dto, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var detalle = await _context.AuditoriaDetalle
                    .Include(d => d.Producto)
                    .Include(d => d.Auditoria)
                    .FirstOrDefaultAsync(d => d.DetalleAuditoriaID == detalleId && d.AuditoriaID == auditoriaId);

                if (detalle == null)
                    throw new InvalidOperationException("Detalle no encontrado");

                if (detalle.Auditoria.EstadoAuditoria == "Cerrada")
                    throw new InvalidOperationException("No se pueden modificar productos en una auditoría cerrada");

                detalle.CantidadEsperada = dto.CantidadEsperada;
                detalle.CantidadRecibida = dto.CantidadRecibida;
                detalle.EstadoItem = dto.EstadoProducto;
                detalle.Observaciones = dto.Observaciones;

                await _context.SaveChangesAsync();

                // Registrar auditoría de acción
                await RegistrarAuditoriaAccionAsync(userId, "ActualizarProducto", "DetalleAuditoria", detalleId);

                await transaction.CommitAsync();

                _logger.LogInformation("Producto actualizado en auditoría: {AuditoriaId} por usuario {UserId}", auditoriaId, userId);

                return MapToDetalleAuditoriaDTO(detalle, detalle.Producto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al actualizar producto en auditoría {AuditoriaId}", auditoriaId);
                throw;
            }
        }

        public async Task DeleteProductoAsync(int auditoriaId, int detalleId, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var detalle = await _context.AuditoriaDetalle
                    .Include(d => d.Auditoria)
                    .FirstOrDefaultAsync(d => d.DetalleAuditoriaID == detalleId && d.AuditoriaID == auditoriaId);

                if (detalle == null)
                    throw new InvalidOperationException("Detalle no encontrado");

                if (detalle.Auditoria.EstadoAuditoria == "Cerrada")
                    throw new InvalidOperationException("No se pueden eliminar productos de una auditoría cerrada");

                _context.AuditoriaDetalle.Remove(detalle);
                await _context.SaveChangesAsync();

                // Registrar auditoría de acción
                await RegistrarAuditoriaAccionAsync(userId, "EliminarProducto", "DetalleAuditoria", detalleId);

                await transaction.CommitAsync();

                _logger.LogInformation("Producto eliminado de auditoría: {AuditoriaId} por usuario {UserId}", auditoriaId, userId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al eliminar producto de auditoría {AuditoriaId}", auditoriaId);
                throw;
            }
        }

        public async Task<bool> ValidarOrdenCompraAsync(string numeroOrdenCompra)
        {
            // Aquí podrías agregar lógica de validación con sistema externo
            return !string.IsNullOrEmpty(numeroOrdenCompra);
        }

        public async Task<string> GenerarNumeroAuditoriaAsync()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month.ToString("D2");
            
            var ultimaAuditoria = await _context.AuditoriasRecepcion
                .Where(a => a.NumeroAuditoria.StartsWith($"AUD-{year}{month}"))
                .OrderByDescending(a => a.NumeroAuditoria)
                .FirstOrDefaultAsync();

            int consecutivo = 1;
            if (ultimaAuditoria != null)
            {
                var partes = ultimaAuditoria.NumeroAuditoria.Split('-');
                if (partes.Length == 3)
                {
                    consecutivo = int.Parse(partes[2]) + 1;
                }
            }

            return $"AUD-{year}{month}-{consecutivo:D5}";
        }

        // Métodos privados auxiliares
        private async Task RegistrarAuditoriaAccionAsync(int userId, string accion, string tabla, int registroId)
        {
            try
            {
                var log = new AuditoriaLog
                {
                    UsuarioID = userId,
                    TipoAccion = accion,
                    TablaAfectada = tabla,
                    RegistroID = registroId,
                    FechaHora = DateTime.Now
                };

                _context.AuditoriaLogs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar auditoría de acción");
                // No lanzar excepción para no afectar el flujo principal
            }
        }

        private AuditoriaDTO MapToAuditoriaDTO(Models.AuditoriaRecepcion auditoria, Proveedor proveedor)
        {
            return new AuditoriaDTO
            {
                AuditoriaID = auditoria.AuditoriaID,
                NumeroAuditoria = auditoria.NumeroAuditoria,
                FechaRecepcion = auditoria.FechaInicio,
                ProveedorID = auditoria.ProveedorID,
                ProveedorNombre = proveedor.NombreProveedor,
                NumeroOrdenCompra = auditoria.OrdenCompraID?.ToString() ?? "",
                NumeroRemito = "",
                Estado = auditoria.EstadoAuditoria,
                FechaCreacion = auditoria.FechaInicio,
                FechaCierre = auditoria.FechaFinalizacion,
                Observaciones = auditoria.ObservacionesGenerales
            };
        }

        private AuditoriaDetalleDTO MapToAuditoriaDetalleDTO(Models.AuditoriaRecepcion auditoria)
        {
            // Implementar mapeo completo con todas las relaciones
            return new AuditoriaDetalleDTO
            {
                AuditoriaID = auditoria.AuditoriaID,
                NumeroAuditoria = auditoria.NumeroAuditoria,
                // ... resto del mapeo
            };
        }

        private DetalleAuditoriaDTO MapToDetalleAuditoriaDTO(AuditoriaDetalle detalle, Producto producto)
        {
            return new DetalleAuditoriaDTO
            {
                DetalleAuditoriaID = detalle.DetalleAuditoriaID,
                AuditoriaID = detalle.AuditoriaID,
                ProductoID = detalle.ProductoID,
                ProductoNombre = producto.Nombre,
                ProductoSKU = producto.SKU,
                ProductoCodigoBarras = producto.CodigoBarras,
                CantidadEsperada = detalle.CantidadEsperada,
                CantidadRecibida = detalle.CantidadRecibida,
                Diferencia = detalle.CantidadEsperada - detalle.CantidadRecibida,
                EstadoProducto = detalle.EstadoItem,
                Observaciones = detalle.Observaciones,
                FechaRegistro = detalle.FechaRegistro
            };
        }
    }
}
using AuditoriaRecepcion.Data;
using AuditoriaRecepcion.DTOs.Incidencia;
using AuditoriaRecepcion.DTOs.Common;
using AuditoriaRecepcion.Models;
using AuditoriaRecepcion.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuditoriaRecepcion.Services.Implementation
{
    public class IncidenciaService : IIncidenciaService
    {
        private readonly AuditoriaRecepcionContext _context;
        private readonly INotificationService _notificationService;
        private readonly ILogger<IncidenciaService> _logger;

        public IncidenciaService(
            AuditoriaRecepcionContext context,
            INotificationService notificationService,
            ILogger<IncidenciaService> logger)
        {
            _context = context;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<PaginatedResult<IncidenciaDTO>> GetIncidenciasAsync(IncidenciaFiltroDTO filtro)
        {
            try
            {
                var query = _context.Incidencias
                    .Include(i => i.Auditoria)
                    .Include(i => i.Producto)
                    .Include(i => i.UsuarioAsignado)
                    .Include(i => i.Evidencias)
                    .AsNoTracking()
                    .AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrEmpty(filtro.TipoIncidencia))
                    query = query.Where(i => i.TipoIncidencia == filtro.TipoIncidencia);

                if (!string.IsNullOrEmpty(filtro.EstadoResolucion))
                    query = query.Where(i => i.EstadoResolucion == filtro.EstadoResolucion);

                if (!string.IsNullOrEmpty(filtro.Prioridad))
                    query = query.Where(i => i.Prioridad == filtro.Prioridad);

                if (filtro.FechaDesde.HasValue)
                    query = query.Where(i => i.FechaDeteccion >= filtro.FechaDesde.Value);

                if (filtro.FechaHasta.HasValue)
                    query = query.Where(i => i.FechaDeteccion <= filtro.FechaHasta.Value);

                if (filtro.AuditoriaId.HasValue)
                    query = query.Where(i => i.AuditoriaID == filtro.AuditoriaId.Value);

                if (filtro.ProveedorId.HasValue)
                    query = query.Where(i => i.Auditoria.ProveedorID == filtro.ProveedorId.Value);

                if (filtro.ProductoId.HasValue)
                    query = query.Where(i => i.ProductoID == filtro.ProductoId.Value);

                if (filtro.UsuarioAsignadoId.HasValue)
                    query = query.Where(i => i.UsuarioAsignadoID == filtro.UsuarioAsignadoId.Value);

                // Contar total
                var totalItems = await query.CountAsync();

                // Ordenar y paginar
                var items = await query
                    .OrderByDescending(i => i.FechaDeteccion)
                    .Skip((filtro.PageNumber - 1) * filtro.PageSize)
                    .Take(filtro.PageSize)
                    .Select(i => new IncidenciaDTO
                    {
                        IncidenciaID = i.IncidenciaID,
                        AuditoriaID = i.AuditoriaID,
                        NumeroAuditoria = i.Auditoria.NumeroAuditoria,
                        DetalleAuditoriaID = i.DetalleAuditoriaID,
                        ProductoID = i.ProductoID,
                        ProductoNombre = i.Producto != null ? i.Producto.Nombre : null,
                        TipoIncidencia = i.TipoIncidencia,
                        Descripcion = i.Descripcion,
                        EstadoResolucion = i.EstadoResolucion,
                        UsuarioAsignadoID = i.UsuarioAsignadoID,
                        UsuarioAsignadoNombre = i.UsuarioAsignado != null ? i.UsuarioAsignado.NombreCompleto : null,
                        FechaDeteccion = i.FechaDeteccion,
                        FechaResolucion = i.FechaResolucion,
                        CantidadEvidencias = i.Evidencias.Count,
                        Prioridad = i.Prioridad
                    })
                    .ToListAsync();

                return new PaginatedResult<IncidenciaDTO>(items, totalItems, filtro.PageNumber, filtro.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener incidencias");
                throw;
            }
        }

        public async Task<IncidenciaDetalleDTO> GetIncidenciaByIdAsync(int id)
        {
            try
            {
                var incidencia = await _context.Incidencias
                    .Include(i => i.Auditoria)
                        .ThenInclude(a => a.Proveedor)
                    .Include(i => i.DetalleAuditoria)
                    .Include(i => i.Producto)
                    .Include(i => i.UsuarioReporto)
                    .Include(i => i.UsuarioAsignado)
                    .Include(i => i.Evidencias)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.IncidenciaID == id);

                if (incidencia == null)
                    return null;

                // Mapear a DTO completo
                return new IncidenciaDetalleDTO
                {
                    IncidenciaID = incidencia.IncidenciaID,
                    // ... mapeo completo de todos los campos
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener incidencia {Id}", id);
                throw;
            }
        }

        public async Task<IncidenciaDTO> CreateIncidenciaAsync(CreateIncidenciaDTO dto, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validar auditoría
                var auditoria = await _context.Auditorias.FindAsync(dto.AuditoriaID);
                if (auditoria == null)
                    throw new InvalidOperationException("Auditoría no encontrada");

                if (auditoria.Estado == "Cerrada")
                    throw new InvalidOperationException("No se pueden agregar incidencias a una auditoría cerrada");

                var incidencia = new Incidencia
                {
                    AuditoriaID = dto.AuditoriaID,
                    DetalleAuditoriaID = dto.DetalleAuditoriaID,
                    ProductoID = dto.ProductoID,
                    TipoIncidencia = dto.TipoIncidencia,
                    Descripcion = dto.Descripcion,
                    EstadoResolucion = "Pendiente",
                    Prioridad = dto.Prioridad,
                    UsuarioReportoID = userId,
                    UsuarioAsignadoID = dto.UsuarioAsignadoID,
                    FechaDeteccion = DateTime.Now
                };

                _context.Incidencias.Add(incidencia);
                await _context.SaveChangesAsync();

                // Registrar auditoría
                await RegistrarAuditoriaAccionAsync(userId, "Crear", "Incidencia", incidencia.IncidenciaID);

                // Enviar notificación
                await _notificationService.NotificarNuevaIncidenciaAsync(incidencia.IncidenciaID);

                await transaction.CommitAsync();

                _logger.LogInformation("Incidencia creada: {Id} por usuario {UserId}", incidencia.IncidenciaID, userId);

                return await MapToIncidenciaDTOAsync(incidencia.IncidenciaID);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al crear incidencia");
                throw;
            }
        }

        public async Task<IncidenciaDTO> UpdateIncidenciaAsync(int id, UpdateIncidenciaDTO dto, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var incidencia = await _context.Incidencias.FindAsync(id);
                if (incidencia == null)
                    return null;

                incidencia.TipoIncidencia = dto.TipoIncidencia;
                incidencia.Descripcion = dto.Descripcion;
                incidencia.AccionCorrectiva = dto.AccionCorrectiva;
                incidencia.Prioridad = dto.Prioridad;
                incidencia.UsuarioAsignadoID = dto.UsuarioAsignadoID;

                await _context.SaveChangesAsync();

                // Registrar auditoría
                await RegistrarAuditoriaAccionAsync(userId, "Actualizar", "Incidencia", id);

                await transaction.CommitAsync();

                _logger.LogInformation("Incidencia actualizada: {Id} por usuario {UserId}", id, userId);

                return await MapToIncidenciaDTOAsync(id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al actualizar incidencia {Id}", id);
                throw;
            }
        }

        public async Task CambiarEstadoAsync(int id, string estadoResolucion, string observaciones, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var incidencia = await _context.Incidencias.FindAsync(id);
                if (incidencia == null)
                    throw new InvalidOperationException("Incidencia no encontrada");

                var estadoAnterior = incidencia.EstadoResolucion;
                incidencia.EstadoResolucion = estadoResolucion;

                if (estadoResolucion == "Resuelta" || estadoResolucion == "Rechazada")
                {
                    incidencia.FechaResolucion = DateTime.Now;
                }

                if (!string.IsNullOrEmpty(observaciones))
                {
                    incidencia.AccionCorrectiva = observaciones;
                }

                await _context.SaveChangesAsync();

                // Registrar auditoría
                await RegistrarAuditoriaAccionAsync(userId, "CambiarEstado", "Incidencia", id);

                // Enviar notificación
                await _notificationService.NotificarCambioEstadoIncidenciaAsync(id, estadoResolucion);

                await transaction.CommitAsync();

                _logger.LogInformation("Estado de incidencia cambiado: {Id} de {EstadoAnterior} a {EstadoNuevo} por usuario {UserId}",
                    id, estadoAnterior, estadoResolucion, userId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al cambiar estado de incidencia {Id}", id);
                throw;
            }
        }

        public async Task<List<IncidenciaDTO>> GetIncidenciasPendientesByUsuarioAsync(int userId)
        {
            try
            {
                var incidencias = await _context.Incidencias
                    .Include(i => i.Auditoria)
                    .Include(i => i.Producto)
                    .Where(i => i.UsuarioAsignadoID == userId &&
                               (i.EstadoResolucion == "Pendiente" || i.EstadoResolucion == "EnProceso"))
                    .OrderByDescending(i => i.Prioridad)
                    .ThenBy(i => i.FechaDeteccion)
                    .Select(i => new IncidenciaDTO
                    {
                        IncidenciaID = i.IncidenciaID,
                        AuditoriaID = i.AuditoriaID,
                        NumeroAuditoria = i.Auditoria.NumeroAuditoria,
                        ProductoNombre = i.Producto != null ? i.Producto.Nombre : null,
                        TipoIncidencia = i.TipoIncidencia,
                        Descripcion = i.Descripcion,
                        EstadoResolucion = i.EstadoResolucion,
                        FechaDeteccion = i.FechaDeteccion,
                        Prioridad = i.Prioridad
                    })
                    .ToListAsync();

                return incidencias;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener incidencias pendientes del usuario {UserId}", userId);
                throw;
            }
        }

        public async Task<ResumenIncidenciasDTO> GetResumenIncidenciasAsync(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            try
            {
                var query = _context.Incidencias.AsQueryable();

                if (fechaDesde.HasValue)
                    query = query.Where(i => i.FechaDeteccion >= fechaDesde.Value);

                if (fechaHasta.HasValue)
                    query = query.Where(i => i.FechaDeteccion <= fechaHasta.Value);

                var incidencias = await query.ToListAsync();

                var totalIncidencias = incidencias.Count;
                var pendientes = incidencias.Count(i => i.EstadoResolucion == "Pendiente");
                var enProceso = incidencias.Count(i => i.EstadoResolucion == "EnProceso");
                var resueltas = incidencias.Count(i => i.EstadoResolucion == "Resuelta");
                var rechazadas = incidencias.Count(i => i.EstadoResolucion == "Rechazada");

                var porcentajeResolucion = totalIncidencias > 0
                    ? (decimal)resueltas / totalIncidencias * 100
                    : 0;

                var incidenciasConTiempo = incidencias
                    .Where(i => i.FechaResolucion.HasValue)
                    .ToList();

                var tiempoPromedioResolucion = incidenciasConTiempo.Any()
                    ? (decimal)incidenciasConTiempo
                        .Average(i => (i.FechaResolucion!.Value - i.FechaDeteccion).TotalHours)
                    : 0;

                var incidenciasPorTipo = incidencias
                    .GroupBy(i => i.TipoIncidencia)
                    .ToDictionary(g => g.Key, g => g.Count());

                var incidenciasPorPrioridad = incidencias
                    .GroupBy(i => i.Prioridad)
                    .ToDictionary(g => g.Key, g => g.Count());

                return new ResumenIncidenciasDTO
                {
                    TotalIncidencias = totalIncidencias,
                    Pendientes = pendientes,
                    EnProceso = enProceso,
                    Resueltas = resueltas,
                    Rechazadas = rechazadas,
                    PorcentajeResolucion = porcentajeResolucion,
                    TiempoPromedioResolucion = tiempoPromedioResolucion,
                    IncidenciasPorTipo = incidenciasPorTipo,
                    IncidenciasPorPrioridad = incidenciasPorPrioridad
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener resumen de incidencias");
                throw;
            }
        }

        public async Task AsignarIncidenciaAsync(int incidenciaId, int usuarioAsignadoId, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var incidencia = await _context.Incidencias.FindAsync(incidenciaId);
                if (incidencia == null)
                    throw new InvalidOperationException("Incidencia no encontrada");

                incidencia.UsuarioAsignadoID = usuarioAsignadoId;

                if (incidencia.EstadoResolucion == "Pendiente")
                {
                    incidencia.EstadoResolucion = "EnProceso";
                }

                await _context.SaveChangesAsync();

                // Registrar auditoría
                await RegistrarAuditoriaAccionAsync(userId, "Asignar", "Incidencia", incidenciaId);

                // Enviar notificación
                await _notificationService.NotificarIncidenciaAsignadaAsync(incidenciaId, usuarioAsignadoId);

                await transaction.CommitAsync();

                _logger.LogInformation("Incidencia {Id} asignada al usuario {UsuarioAsignadoId} por usuario {UserId}",
                    incidenciaId, usuarioAsignadoId, userId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al asignar incidencia {Id}", incidenciaId);
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

        private async Task<IncidenciaDTO> MapToIncidenciaDTOAsync(int incidenciaId)
        {
            var incidencia = await _context.Incidencias
                .Include(i => i.Auditoria)
                .Include(i => i.Producto)
                .Include(i => i.UsuarioAsignado)
                .Include(i => i.Evidencias)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.IncidenciaID == incidenciaId);

            if (incidencia == null)
                return null;

            return new IncidenciaDTO
            {
                IncidenciaID = incidencia.IncidenciaID,
                AuditoriaID = incidencia.AuditoriaID,
                NumeroAuditoria = incidencia.Auditoria.NumeroAuditoria,
                DetalleAuditoriaID = incidencia.DetalleAuditoriaID,
                ProductoID = incidencia.ProductoID,
                ProductoNombre = incidencia.Producto?.Nombre,
                TipoIncidencia = incidencia.TipoIncidencia,
                Descripcion = incidencia.Descripcion,
                EstadoResolucion = incidencia.EstadoResolucion,
                UsuarioAsignadoID = incidencia.UsuarioAsignadoID,
                UsuarioAsignadoNombre = incidencia.UsuarioAsignado?.NombreCompleto,
                FechaDeteccion = incidencia.FechaDeteccion,
                FechaResolucion = incidencia.FechaResolucion,
                CantidadEvidencias = incidencia.Evidencias.Count,
                Prioridad = incidencia.Prioridad
            };
        }
    }
}
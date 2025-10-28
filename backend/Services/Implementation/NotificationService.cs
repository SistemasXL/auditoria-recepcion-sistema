using AuditoriaRecepcion.Data;
using AuditoriaRecepcion.DTOs.Dashboard;
using AuditoriaRecepcion.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuditoriaRecepcion.Services.Implementation
{
    public class NotificationService : INotificationService
    {
        private readonly AuditoriaRecepcionContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            AuditoriaRecepcionContext context,
            IEmailService emailService,
            ILogger<NotificationService> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task NotificarNuevaIncidenciaAsync(int incidenciaId)
        {
            try
            {
                var incidencia = await _context.Incidencias
                    .Include(i => i.Auditoria)
                        .ThenInclude(a => a.Proveedor)
                    .Include(i => i.Producto)
                    .FirstOrDefaultAsync(i => i.IncidenciaID == incidenciaId);

                if (incidencia == null)
                    return;

                // Obtener usuarios con rol "Comprador" y "JefeLogistica"
                var usuarios = await _context.Usuarios
                    .Where(u => (u.Rol == "Comprador" || u.Rol == "JefeLogistica") && u.Activo)
                    .ToListAsync();

                // Crear alerta para cada usuario
                foreach (var usuario in usuarios)
                {
                    var alerta = new AlertaDTO
                    {
                        TipoAlerta = "Incidencia",
                        Prioridad = incidencia.Prioridad,
                        Titulo = $"Nueva Incidencia: {incidencia.TipoIncidencia}",
                        Mensaje = $"Se detectó una incidencia en la auditoría {incidencia.Auditoria.NumeroAuditoria} del proveedor {incidencia.Auditoria.Proveedor.RazonSocial}",
                        FechaAlerta = DateTime.Now,
                        Leida = false,
                        UrlAccion = $"/incidencias/{incidenciaId}",
                        DatosAdicionales = new Dictionary<string, object>
                        {
                            { "IncidenciaID", incidenciaId },
                            { "AuditoriaID", incidencia.AuditoriaID },
                            { "TipoIncidencia", incidencia.TipoIncidencia }
                        }
                    };

                    await EnviarAlertaAsync(alerta, new List<int> { usuario.UsuarioID });

                    // Enviar email
                    await _emailService.SendIncidenciaAlertEmailAsync(
                        usuario.Email,
                        incidenciaId,
                        incidencia.Descripcion);
                }

                _logger.LogInformation("Notificaciones enviadas para nueva incidencia: {IncidenciaId}", incidenciaId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al notificar nueva incidencia {IncidenciaId}", incidenciaId);
            }
        }

        public async Task NotificarCambioEstadoIncidenciaAsync(int incidenciaId, string nuevoEstado)
        {
            try
            {
                var incidencia = await _context.Incidencias
                    .Include(i => i.Auditoria)
                    .Include(i => i.UsuarioReporto)
                    .Include(i => i.UsuarioAsignado)
                    .FirstOrDefaultAsync(i => i.IncidenciaID == incidenciaId);

                if (incidencia == null)
                    return;

                var usuariosNotificar = new List<int>();

                // Notificar al usuario que reportó
                if (incidencia.UsuarioReportoID.HasValue)
                    usuariosNotificar.Add(incidencia.UsuarioReportoID.Value);

                // Notificar al usuario asignado si es diferente
                if (incidencia.UsuarioAsignadoID.HasValue &&
                    incidencia.UsuarioAsignadoID != incidencia.UsuarioReportoID)
                    usuariosNotificar.Add(incidencia.UsuarioAsignadoID.Value);

                var mensajePrioridad = nuevoEstado switch
                {
                    "Resuelta" => "Baja",
                    "Rechazada" => "Media",
                    "EnProceso" => "Media",
                    _ => "Baja"
                };

                var alerta = new AlertaDTO
                {
                    TipoAlerta = "CambioEstadoIncidencia",
                    Prioridad = mensajePrioridad,
                    Titulo = $"Cambio de Estado: Incidencia #{incidenciaId}",
                    Mensaje = $"La incidencia ha cambiado a estado: {nuevoEstado}",
                    FechaAlerta = DateTime.Now,
                    Leida = false,
                    UrlAccion = $"/incidencias/{incidenciaId}",
                    DatosAdicionales = new Dictionary<string, object>
                    {
                        { "IncidenciaID", incidenciaId },
                        { "NuevoEstado", nuevoEstado }
                    }
                };

                await EnviarAlertaAsync(alerta, usuariosNotificar);

                _logger.LogInformation("Notificación de cambio de estado enviada para incidencia: {IncidenciaId} a estado {Estado}",
                    incidenciaId, nuevoEstado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al notificar cambio de estado de incidencia {IncidenciaId}", incidenciaId);
            }
        }

        public async Task NotificarIncidenciaAsignadaAsync(int incidenciaId, int usuarioAsignadoId)
        {
            try
            {
                var incidencia = await _context.Incidencias
                    .Include(i => i.Auditoria)
                    .Include(i => i.Producto)
                    .FirstOrDefaultAsync(i => i.IncidenciaID == incidenciaId);

                var usuario = await _context.Usuarios.FindAsync(usuarioAsignadoId);

                if (incidencia == null || usuario == null)
                    return;

                var alerta = new AlertaDTO
                {
                    TipoAlerta = "IncidenciaAsignada",
                    Prioridad = incidencia.Prioridad,
                    Titulo = "Incidencia Asignada",
                    Mensaje = $"Se te ha asignado una incidencia: {incidencia.TipoIncidencia}",
                    FechaAlerta = DateTime.Now,
                    Leida = false,
                    UrlAccion = $"/incidencias/{incidenciaId}",
                    DatosAdicionales = new Dictionary<string, object>
                    {
                        { "IncidenciaID", incidenciaId },
                        { "TipoIncidencia", incidencia.TipoIncidencia }
                    }
                };

                await EnviarAlertaAsync(alerta, new List<int> { usuarioAsignadoId });

                // Enviar email
                await _emailService.SendIncidenciaAlertEmailAsync(
                    usuario.Email,
                    incidenciaId,
                    $"Se te ha asignado: {incidencia.Descripcion}");

                _logger.LogInformation("Notificación de asignación enviada para incidencia: {IncidenciaId} a usuario {UsuarioId}",
                    incidenciaId, usuarioAsignadoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al notificar asignación de incidencia {IncidenciaId}", incidenciaId);
            }
        }

        public async Task NotificarAuditoriaVencidaAsync(int auditoriaId)
        {
            try
            {
                var auditoria = await _context.Auditorias
                    .Include(a => a.UsuarioCreacion)
                    .FirstOrDefaultAsync(a => a.AuditoriaID == auditoriaId);

                if (auditoria == null)
                    return;

                // Obtener jefes de logística
                var jefes = await _context.Usuarios
                    .Where(u => u.Rol == "JefeLogistica" && u.Activo)
                    .ToListAsync();

                var usuariosNotificar = jefes.Select(j => j.UsuarioID).ToList();

                // Agregar el usuario que creó la auditoría
                if (auditoria.UsuarioCreacionID.HasValue &&
                    !usuariosNotificar.Contains(auditoria.UsuarioCreacionID.Value))
                {
                    usuariosNotificar.Add(auditoria.UsuarioCreacionID.Value);
                }

                var alerta = new AlertaDTO
                {
                    TipoAlerta = "AuditoriaVencida",
                    Prioridad = "Alta",
                    Titulo = "Auditoría Vencida",
                    Mensaje = $"La auditoría {auditoria.NumeroAuditoria} está abierta desde hace más de 7 días",
                    FechaAlerta = DateTime.Now,
                    Leida = false,
                    UrlAccion = $"/auditorias/{auditoriaId}",
                    DatosAdicionales = new Dictionary<string, object>
                    {
                        { "AuditoriaID", auditoriaId },
                        { "NumeroAuditoria", auditoria.NumeroAuditoria }
                    }
                };

                await EnviarAlertaAsync(alerta, usuariosNotificar);

                _logger.LogInformation("Notificación de auditoría vencida enviada: {AuditoriaId}", auditoriaId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al notificar auditoría vencida {AuditoriaId}", auditoriaId);
            }
        }

        public async Task EnviarAlertaAsync(AlertaDTO alerta, List<int> usuariosDestino)
        {
            try
            {
                // Aquí podrías implementar el guardado de alertas en la BD si tienes una tabla para ello
                // Por ahora, solo logueamos la alerta

                _logger.LogInformation("Alerta enviada: {Titulo} a {CantidadUsuarios} usuarios",
                    alerta.Titulo, usuariosDestino.Count);

                // Si implementas SignalR o WebSockets, aquí enviarías las notificaciones en tiempo real
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar alerta");
            }
        }

        public async Task<List<AlertaDTO>> GetNotificacionesUsuarioAsync(int userId, bool soloNoLeidas = true)
        {
            try
            {
                // Implementación simplificada - en producción deberías tener una tabla de Notificaciones
                var alertas = new List<AlertaDTO>();

                // Obtener incidencias asignadas al usuario
                var incidencias = await _context.Incidencias
                    .Include(i => i.Auditoria)
                    .Where(i => i.UsuarioAsignadoID == userId &&
                               (i.EstadoResolucion == "Pendiente" || i.EstadoResolucion == "EnProceso"))
                    .OrderByDescending(i => i.FechaDeteccion)
                    .Take(10)
                    .ToListAsync();

                foreach (var incidencia in incidencias)
                {
                    alertas.Add(new AlertaDTO
                    {
                        AlertaID = incidencia.IncidenciaID,
                        TipoAlerta = "IncidenciaPendiente",
                        Prioridad = incidencia.Prioridad,
                        Titulo = $"Incidencia Pendiente: {incidencia.TipoIncidencia}",
                        Mensaje = incidencia.Descripcion,
                        FechaAlerta = incidencia.FechaDeteccion,
                        Leida = false,
                        UrlAccion = $"/incidencias/{incidencia.IncidenciaID}"
                    });
                }

                return alertas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener notificaciones del usuario {UserId}", userId);
                throw;
            }
        }

        public async Task MarcarNotificacionLeidaAsync(int alertaId, int userId)
        {
            try
            {
                // Implementación simplificada - en producción actualizarías el estado en la tabla de Notificaciones
                _logger.LogInformation("Notificación {AlertaId} marcada como leída por usuario {UserId}", alertaId, userId);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar notificación {AlertaId} como leída", alertaId);
            }
        }
    }
}
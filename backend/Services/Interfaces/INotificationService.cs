using AuditoriaRecepcion.DTOs.Dashboard;

namespace AuditoriaRecepcion.Services.Interfaces
{
    public interface INotificationService
    {
        Task NotificarNuevaIncidenciaAsync(int incidenciaId);
        Task NotificarCambioEstadoIncidenciaAsync(int incidenciaId, string nuevoEstado);
        Task NotificarIncidenciaAsignadaAsync(int incidenciaId, int usuarioAsignadoId);
        Task NotificarAuditoriaVencidaAsync(int auditoriaId);
        Task EnviarAlertaAsync(AlertaDTO alerta, List<int> usuariosDestino);
        Task<List<AlertaDTO>> GetNotificacionesUsuarioAsync(int userId, bool soloNoLeidas = true);
        Task MarcarNotificacionLeidaAsync(int alertaId, int userId);
    }
}
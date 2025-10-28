namespace AuditoriaRecepcion.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task SendEmailAsync(List<string> to, string subject, string body, bool isHtml = true);
        Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string attachmentName);
        Task SendWelcomeEmailAsync(string to, string username, string temporaryPassword);
        Task SendPasswordResetEmailAsync(string to, string username, string newPassword);
        Task SendIncidenciaAlertEmailAsync(string to, int incidenciaId, string descripcion);
        Task SendReporteEmailAsync(List<string> to, string reportName, byte[] reportFile, string fileName);
    }
}
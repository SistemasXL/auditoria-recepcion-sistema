using AuditoriaRecepcion.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace AuditoriaRecepcion.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _smtpHost = _configuration["Email:SmtpHost"];
            _smtpPort = int.Parse(_configuration["Email:SmtpPort"]);
            _smtpUser = _configuration["Email:SmtpUser"];
            _smtpPassword = _configuration["Email:SmtpPassword"];
            _fromEmail = _configuration["Email:FromEmail"];
            _fromName = _configuration["Email:FromName"];
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            await SendEmailAsync(new List<string> { to }, subject, body, isHtml);
        }

        public async Task SendEmailAsync(List<string> to, string subject, string body, bool isHtml = true)
        {
            try
            {
                using var message = new MailMessage();
                message.From = new MailAddress(_fromEmail, _fromName);
                
                foreach (var recipient in to)
                {
                    message.To.Add(new MailAddress(recipient));
                }

                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = isHtml;

                using var client = new SmtpClient(_smtpHost, _smtpPort);
                client.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                client.EnableSsl = true;

                await client.SendMailAsync(message);

                _logger.LogInformation("Email enviado a: {Recipients}", string.Join(", ", to));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar email a: {Recipients}", string.Join(", ", to));
                throw;
            }
        }

        public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string attachmentName)
        {
            try
            {
                using var message = new MailMessage();
                message.From = new MailAddress(_fromEmail, _fromName);
                message.To.Add(new MailAddress(to));
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                using var stream = new MemoryStream(attachment);
                message.Attachments.Add(new Attachment(stream, attachmentName));

                using var client = new SmtpClient(_smtpHost, _smtpPort);
                client.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                client.EnableSsl = true;

                await client.SendMailAsync(message);

                _logger.LogInformation("Email con adjunto enviado a: {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar email con adjunto a: {To}", to);
                throw;
            }
        }

        public async Task SendWelcomeEmailAsync(string to, string username, string temporaryPassword)
        {
            var subject = "Bienvenido al Sistema de Auditoría de Recepción";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Bienvenido al Sistema de Auditoría de Recepción</h2>
                    <p>Hola,</p>
                    <p>Tu cuenta ha sido creada exitosamente.</p>
                    <p><strong>Usuario:</strong> {username}</p>
                    <p><strong>Contraseña temporal:</strong> {temporaryPassword}</p>
                    <p><strong style='color: red;'>Por seguridad, debes cambiar tu contraseña en el primer inicio de sesión.</strong></p>
                    <p>Puedes acceder al sistema en: <a href='http://localhost:5000'>http://localhost:5000</a></p>
                    <br/>
                    <p>Saludos,<br/>Equipo de Auditoría de Recepción</p>
                </body>
                </html>";

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string to, string username, string newPassword)
        {
            var subject = "Restablecimiento de Contraseña";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Restablecimiento de Contraseña</h2>
                    <p>Hola {username},</p>
                    <p>Tu contraseña ha sido restablecida.</p>
                    <p><strong>Nueva contraseña temporal:</strong> {newPassword}</p>
                    <p><strong style='color: red;'>Por seguridad, te recomendamos cambiar tu contraseña inmediatamente después de iniciar sesión.</strong></p>
                    <p>Puedes acceder al sistema en: <a href='http://localhost:5000'>http://localhost:5000</a></p>
                    <br/>
                    <p>Si no solicitaste este cambio, contacta al administrador inmediatamente.</p>
                    <br/>
                    <p>Saludos,<br/>Equipo de Auditoría de Recepción</p>
                </body>
                </html>";

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendIncidenciaAlertEmailAsync(string to, int incidenciaId, string descripcion)
        {
            var subject = $"Alerta: Nueva Incidencia #{incidenciaId}";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #d9534f;'>Nueva Incidencia Detectada</h2>
                    <p>Se ha detectado una nueva incidencia que requiere tu atención.</p>
                    <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #d9534f; margin: 20px 0;'>
                        <p><strong>ID Incidencia:</strong> #{incidenciaId}</p>
                        <p><strong>Descripción:</strong> {descripcion}</p>
                    </div>
                    <p>Por favor, revisa los detalles en el sistema: <a href='http://localhost:5000/incidencias/{incidenciaId}'>Ver Incidencia</a></p>
                    <br/>
                    <p>Saludos,<br/>Sistema de Auditoría de Recepción</p>
                </body>
                </html>";

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendReporteEmailAsync(List<string> to, string reportName, byte[] reportFile, string fileName)
        {
            var subject = $"Reporte: {reportName}";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Reporte Generado</h2>
                    <p>Se adjunta el reporte solicitado: <strong>{reportName}</strong></p>
                    <p>Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                    <br/>
                    <p>Saludos,<br/>Sistema de Auditoría de Recepción</p>
                </body>
                </html>";

            foreach (var recipient in to)
            {
                await SendEmailWithAttachmentAsync(recipient, subject, body, reportFile, fileName);
            }
        }
    }
}
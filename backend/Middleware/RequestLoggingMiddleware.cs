using System.Diagnostics;
using System.Text;

namespace AuditoriaRecepcion.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Generar ID único para la request
            var requestId = Guid.NewGuid().ToString();
            context.Items["RequestId"] = requestId;

            // Capturar información de la request
            var request = context.Request;
            var stopwatch = Stopwatch.StartNew();

            // Log de entrada
            _logger.LogInformation(
                "Request Started: {RequestId} {Method} {Path} from {RemoteIp}",
                requestId,
                request.Method,
                request.Path,
                context.Connection.RemoteIpAddress);

            // Capturar body de la request si es POST/PUT (para auditoría)
            string requestBody = null;
            if (request.Method == "POST" || request.Method == "PUT")
            {
                request.EnableBuffering();
                using (var reader = new StreamReader(
                    request.Body,
                    encoding: Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    leaveOpen: true))
                {
                    requestBody = await reader.ReadToEndAsync();
                    request.Body.Position = 0;
                }
            }

            // Guardar el response stream original
            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                try
                {
                    await _next(context);
                }
                finally
                {
                    stopwatch.Stop();

                    // Log de salida
                    _logger.LogInformation(
                        "Request Completed: {RequestId} {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms",
                        requestId,
                        request.Method,
                        request.Path,
                        context.Response.StatusCode,
                        stopwatch.ElapsedMilliseconds);

                    // Si hubo error, log adicional
                    if (context.Response.StatusCode >= 400)
                    {
                        _logger.LogWarning(
                            "Request Failed: {RequestId} {Method} {Path} - Status: {StatusCode}",
                            requestId,
                            request.Method,
                            request.Path,
                            context.Response.StatusCode);
                    }

                    // Copiar el response body de vuelta al stream original
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
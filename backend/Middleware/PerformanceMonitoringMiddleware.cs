using System.Diagnostics;

namespace AuditoriaRecepcion.Middleware
{
    public class PerformanceMonitoringMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceMonitoringMiddleware> _logger;
        private const int SlowRequestThresholdMs = 3000; // 3 segundos

        public PerformanceMonitoringMiddleware(
            RequestDelegate next,
            ILogger<PerformanceMonitoringMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                // Log de requests lentas
                if (stopwatch.ElapsedMilliseconds > SlowRequestThresholdMs)
                {
                    _logger.LogWarning(
                        "Slow Request Detected: {Method} {Path} took {ElapsedMilliseconds}ms - User: {User}",
                        context.Request.Method,
                        context.Request.Path,
                        stopwatch.ElapsedMilliseconds,
                        context.Items["Username"] ?? "Anonymous");
                }

                // Agregar header de performance
                context.Response.Headers.Add("X-Response-Time-Ms", stopwatch.ElapsedMilliseconds.ToString());
            }
        }
    }

    public static class PerformanceMonitoringMiddlewareExtensions
    {
        public static IApplicationBuilder UsePerformanceMonitoring(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PerformanceMonitoringMiddleware>();
        }
    }
}

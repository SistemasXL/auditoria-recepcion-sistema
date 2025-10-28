using AuditoriaRecepcion.Data;
using AuditoriaRecepcion.DTOs.Dashboard;
using AuditoriaRecepcion.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuditoriaRecepcion.Services.Implementation
{
    public class DashboardService : IDashboardService
    {
        private readonly AuditoriaRecepcionContext _context;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            AuditoriaRecepcionContext context,
            ILogger<DashboardService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<KPIsGeneralesDTO> GetKPIsGeneralesAsync(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            try
            {
                fechaDesde ??= DateTime.Now.AddMonths(-1);
                fechaHasta ??= DateTime.Now;

                var auditorias = await _context.Auditorias
                    .Where(a => a.FechaCreacion >= fechaDesde && a.FechaCreacion <= fechaHasta)
                    .ToListAsync();

                var incidencias = await _context.Incidencias
                    .Where(i => i.FechaDeteccion >= fechaDesde && i.FechaDeteccion <= fechaHasta)
                    .ToListAsync();

                var totalAuditorias = auditorias.Count;
                var auditoriasAbiertas = auditorias.Count(a => a.Estado == "Abierta");
                var auditoriasCerradas = auditorias.Count(a => a.Estado == "Cerrada");
                var tasaCierre = totalAuditorias > 0 ? (decimal)auditoriasCerradas / totalAuditorias * 100 : 0;

                var totalIncidencias = incidencias.Count;
                var incidenciasPendientes = incidencias.Count(i => i.EstadoResolucion == "Pendiente");
                var incidenciasResueltas = incidencias.Count(i => i.EstadoResolucion == "Resuelta");
                var tasaResolucion = totalIncidencias > 0 ? (decimal)incidenciasResueltas / totalIncidencias * 100 : 0;

                var incidenciasConTiempo = incidencias.Where(i => i.FechaResolucion.HasValue).ToList();
                var tiempoPromedioResolucion = incidenciasConTiempo.Any()
                    ? (decimal)incidenciasConTiempo.Average(i => (i.FechaResolucion!.Value - i.FechaDeteccion).TotalHours)
                    : 0;

                var detalles = await _context.DetallesAuditoria
                    .Include(d => d.Auditoria)
                    .Where(d => d.Auditoria.FechaCreacion >= fechaDesde && d.Auditoria.FechaCreacion <= fechaHasta)
                    .ToListAsync();

                var totalProductosAuditados = detalles.Count;
                var productosConIncidencias = detalles.Count(d => d.EstadoProducto != "Bueno");
                var porcentajeProductosOK = totalProductosAuditados > 0
                    ? (decimal)(totalProductosAuditados - productosConIncidencias) / totalProductosAuditados * 100
                    : 100;

                var proveedoresActivos = await _context.Proveedores.CountAsync(p => p.Activo);
                var proveedoresConIncidencias = await _context.Auditorias
                    .Include(a => a.Incidencias)
                    .Where(a => a.FechaCreacion >= fechaDesde && a.FechaCreacion <= fechaHasta && a.Incidencias.Any())
                    .Select(a => a.ProveedorID)
                    .Distinct()
                    .CountAsync();

                // Cálculo de cambios respecto al período anterior (simplificado)
                var duracionPeriodo = (fechaHasta.Value - fechaDesde.Value).Days;
                var fechaDesdePeriodoAnterior = fechaDesde.Value.AddDays(-duracionPeriodo);
                var fechaHastaPeriodoAnterior = fechaDesde.Value.AddDays(-1);

                var auditoriasAnterior = await _context.Auditorias
                    .CountAsync(a => a.FechaCreacion >= fechaDesdePeriodoAnterior && a.FechaCreacion <= fechaHastaPeriodoAnterior);

                var cambioAuditorias = auditoriasAnterior > 0
                    ? ((decimal)totalAuditorias - auditoriasAnterior) / auditoriasAnterior * 100
                    : 0;

                return new KPIsGeneralesDTO
                {
                    TotalAuditorias = totalAuditorias,
                    AuditoriasAbiertas = auditoriasAbiertas,
                    AuditoriasCerradas = auditoriasCerradas,
                    TasaCierre = tasaCierre,
                    TotalIncidencias = totalIncidencias,
                    IncidenciasPendientes = incidenciasPendientes,
                    IncidenciasResueltas = incidenciasResueltas,
                    TasaResolucion = tasaResolucion,
                    TiempoPromedioResolucion = tiempoPromedioResolucion,
                    TotalProductosAuditados = totalProductosAuditados,
                    ProductosConIncidencias = productosConIncidencias,
                    PorcentajeProductosOK = porcentajeProductosOK,
                    TotalProveedoresActivos = proveedoresActivos,
                    ProveedoresConIncidencias = proveedoresConIncidencias,
                    CambioAuditorias = cambioAuditorias,
                    CambioIncidencias = 0, // TODO: Implementar cálculo
                    CambioTiempoResolucion = 0 // TODO: Implementar cálculo
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener KPIs generales");
                throw;
            }
        }

        public async Task<MetricasAuditoriasDTO> GetMetricasAuditoriasAsync(DateTime? fechaDesde, DateTime? fechaHasta, string agrupacion)
        {
            try
            {
                fechaDesde ??= DateTime.Now.AddMonths(-1);
                fechaHasta ??= DateTime.Now;

                var auditorias = await _context.Auditorias
                    .Include(a => a.Detalles)
                    .Where(a => a.FechaCreacion >= fechaDesde && a.FechaCreacion <= fechaHasta)
                    .ToListAsync();

                var totalAuditorias = auditorias.Count;
                var auditoriasAbiertas = auditorias.Count(a => a.Estado == "Abierta");
                var auditoriasCerradas = auditorias.Count(a => a.Estado == "Cerrada");

                var promedioProductosPorAuditoria = auditorias.Any()
                    ? (decimal)auditorias.Average(a => a.Detalles.Count)
                    : 0;

                var auditoriasCerradasConTiempo = auditorias.Where(a => a.FechaCierre.HasValue).ToList();
                var promedioTiempoCierre = auditoriasCerradasConTiempo.Any()
                    ? (decimal)auditoriasCerradasConTiempo.Average(a => (a.FechaCierre!.Value - a.FechaCreacion).TotalHours)
                    : 0;

                // Serie temporal (simplificada)
                var serieTemporal = new List<SerieTemporalDTO>();
                // TODO: Implementar agrupación por día/semana/mes

                var distribucionPorEstado = auditorias
                    .GroupBy(a => a.Estado)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Top usuarios
                var topUsuarios = auditorias
                    .Where(a => a.UsuarioCreacionID.HasValue)
                    .GroupBy(a => new { a.UsuarioCreacionID, a.UsuarioCreacion.NombreCompleto })
                    .Select(g => new TopUsuarioAuditoriasDTO
                    {
                        UsuarioID = g.Key.UsuarioCreacionID.Value,
                        NombreCompleto = g.Key.NombreCompleto,
                        CantidadAuditorias = g.Count(),
                        Porcentaje = totalAuditorias > 0 ? (decimal)g.Count() / totalAuditorias * 100 : 0
                    })
                    .OrderByDescending(u => u.CantidadAuditorias)
                    .Take(5)
                    .ToList();

                return new MetricasAuditoriasDTO
                {
                    TotalAuditorias = totalAuditorias,
                    AuditoriasAbiertas = auditoriasAbiertas,
                    AuditoriasCerradas = auditoriasCerradas,
                    PromedioProductosPorAuditoria = promedioProductosPorAuditoria,
                    PromedioTiempoCierre = promedioTiempoCierre,
                    SerieTemporal = serieTemporal,
                    DistribucionPorEstado = distribucionPorEstado,
                    TopUsuarios = topUsuarios
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener métricas de auditorías");
                throw;
            }
        }

        public async Task<MetricasIncidenciasDTO> GetMetricasIncidenciasAsync(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            try
            {
                fechaDesde ??= DateTime.Now.AddMonths(-1);
                fechaHasta ??= DateTime.Now;

                var incidencias = await _context.Incidencias
                    .Where(i => i.FechaDeteccion >= fechaDesde && i.FechaDeteccion <= fechaHasta)
                    .ToListAsync();

                var totalIncidencias = incidencias.Count;
                var pendientes = incidencias.Count(i => i.EstadoResolucion == "Pendiente");
                var enProceso = incidencias.Count(i => i.EstadoResolucion == "EnProceso");
                var resueltas = incidencias.Count(i => i.EstadoResolucion == "Resuelta");
                var rechazadas = incidencias.Count(i => i.EstadoResolucion == "Rechazada");

                var tasaResolucion = totalIncidencias > 0 ? (decimal)resueltas / totalIncidencias * 100 : 0;

                var incidenciasConTiempo = incidencias.Where(i => i.FechaResolucion.HasValue).ToList();
                var tiempoPromedioResolucion = incidenciasConTiempo.Any()
                    ? (decimal)incidenciasConTiempo.Average(i => (i.FechaResolucion!.Value - i.FechaDeteccion).TotalHours)
                    : 0;

                var tiempoMedianoResolucion = incidenciasConTiempo.Any()
                    ? CalcularMediana(incidenciasConTiempo.Select(i => (i.FechaResolucion!.Value - i.FechaDeteccion).TotalHours).ToList())
                    : 0;

                var distribucionPorTipo = incidencias
                    .GroupBy(i => i.TipoIncidencia)
                    .ToDictionary(g => g.Key, g => g.Count());

                var distribucionPorPrioridad = incidencias
                    .GroupBy(i => i.Prioridad)
                    .ToDictionary(g => g.Key, g => g.Count());

                return new MetricasIncidenciasDTO
                {
                    TotalIncidencias = totalIncidencias,
                    IncidenciasPendientes = pendientes,
                    IncidenciasEnProceso = enProceso,
                    IncidenciasResueltas = resueltas,
                    IncidenciasRechazadas = rechazadas,
                    TasaResolucion = tasaResolucion,
                    TiempoPromedioResolucion = tiempoPromedioResolucion,
                    TiempoMedianoResolucion = tiempoMedianoResolucion,
                    DistribucionPorTipo = distribucionPorTipo,
                    DistribucionPorPrioridad = distribucionPorPrioridad,
                    SerieTemporal = new List<SerieTemporalDTO>() // TODO: Implementar
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener métricas de incidencias");
                throw;
            }
        }

        public async Task<List<TopProveedorIncidenciasDTO>> GetTopProveedoresIncidenciasAsync(DateTime? fechaDesde, DateTime? fechaHasta, int top)
        {
            try
            {
                fechaDesde ??= DateTime.Now.AddMonths(-3);
                fechaHasta ??= DateTime.Now;

                var resultado = await _context.Auditorias
                    .Include(a => a.Proveedor)
                    .Include(a => a.Incidencias)
                    .Where(a => a.FechaRecepcion >= fechaDesde && a.FechaRecepcion <= fechaHasta)
                    .GroupBy(a => new { a.ProveedorID, a.Proveedor.RazonSocial })
                    .Select(g => new TopProveedorIncidenciasDTO
                    {
                        ProveedorID = g.Key.ProveedorID,
                        RazonSocial = g.Key.RazonSocial,
                        TotalAuditorias = g.Count(),
                        TotalIncidencias = g.Sum(a => a.Incidencias.Count),
                        PorcentajeIncidencias = g.Count() > 0 ? (decimal)g.Sum(a => a.Incidencias.Count) / g.Count() * 100 : 0,
                        TiempoPromedioResolucion = 0, // TODO: Implementar cálculo real
                        Calificacion = "Regular" // TODO: Implementar lógica de calificación
                    })
                    .OrderByDescending(p => p.TotalIncidencias)
                    .Take(top)
                    .ToListAsync();

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener top proveedores con incidencias");
                throw;
            }
        }

        public async Task<List<TopProductoIncidenciasDTO>> GetTopProductosIncidenciasAsync(DateTime? fechaDesde, DateTime? fechaHasta, int top)
        {
            try
            {
                fechaDesde ??= DateTime.Now.AddMonths(-3);
                fechaHasta ??= DateTime.Now;

                var resultado = await _context.Incidencias
                    .Include(i => i.Producto)
                    .Where(i => i.FechaDeteccion >= fechaDesde && i.FechaDeteccion <= fechaHasta && i.ProductoID.HasValue)
                    .GroupBy(i => new { i.ProductoID, i.Producto.Nombre, i.Producto.SKU })
                    .Select(g => new TopProductoIncidenciasDTO
                    {
                        ProductoID = g.Key.ProductoID.Value,
                        Nombre = g.Key.Nombre,
                        SKU = g.Key.SKU,
                        TotalRecepciones = 0, // TODO: Calcular de DetallesAuditoria
                        TotalIncidencias = g.Count(),
                        PorcentajeIncidencias = 0, // TODO: Calcular correctamente
                        IncidenciasPorTipo = g.GroupBy(i => i.TipoIncidencia).ToDictionary(t => t.Key, t => t.Count())
                    })
                    .OrderByDescending(p => p.TotalIncidencias)
                    .Take(top)
                    .ToListAsync();

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener top productos con incidencias");
                throw;
            }
        }

        public async Task<List<DistribucionIncidenciasDTO>> GetDistribucionIncidenciasAsync(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            try
            {
                fechaDesde ??= DateTime.Now.AddMonths(-1);
                fechaHasta ??= DateTime.Now;

                var incidencias = await _context.Incidencias
                    .Where(i => i.FechaDeteccion >= fechaDesde && i.FechaDeteccion <= fechaHasta)
                    .ToListAsync();

                var total = incidencias.Count;

                var colores = new Dictionary<string, string>
                {
                    { "Faltante", "#dc3545" },
                    { "Excedente", "#28a745" },
                    { "Dañado", "#ffc107" },
                    { "Defectuoso", "#fd7e14" },
                    { "Incorrecto", "#6f42c1" },
                    { "DocumentacionIncompleta", "#17a2b8" },
                    { "Otro", "#6c757d" }
                };

                var distribucion = incidencias
                    .GroupBy(i => i.TipoIncidencia)
                    .Select(g => new DistribucionIncidenciasDTO
                    {
                        TipoIncidencia = g.Key,
                        Cantidad = g.Count(),
                        Porcentaje = total > 0 ? (decimal)g.Count() / total * 100 : 0,
                        Color = colores.ContainsKey(g.Key) ? colores[g.Key] : "#6c757d"
                    })
                    .OrderByDescending(d => d.Cantidad)
                    .ToList();

                return distribucion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener distribución de incidencias");
                throw;
            }
        }

        public async Task<List<TendenciaAuditoriasDTO>> GetTendenciaAuditoriasAsync(DateTime? fechaDesde, DateTime? fechaHasta, string agrupacion)
        {
            // TODO: Implementar agrupación por día/semana/mes
            return await Task.FromResult(new List<TendenciaAuditoriasDTO>());
        }

        public async Task<MetricasTiempoResolucionDTO> GetMetricasTiempoResolucionAsync(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            // TODO: Implementar cálculo completo de métricas de tiempo
            return await Task.FromResult(new MetricasTiempoResolucionDTO());
        }

        public async Task<ResumenDiarioDTO> GetResumenDiarioAsync(DateTime fecha)
        {
            try
            {
                var inicioFecha = fecha.Date;
                var finFecha = fecha.Date.AddDays(1);

                var auditoriasCreadas = await _context.Auditorias
                    .CountAsync(a => a.FechaCreacion >= inicioFecha && a.FechaCreacion < finFecha);

                var auditoriasCerradas = await _context.Auditorias
                    .CountAsync(a => a.FechaCierre.HasValue && a.FechaCierre >= inicioFecha && a.FechaCierre < finFecha);

                var auditoriasAbiertas = await _context.Auditorias
                    .CountAsync(a => a.Estado == "Abierta");

                var incidenciasNuevas = await _context.Incidencias
                    .CountAsync(i => i.FechaDeteccion >= inicioFecha && i.FechaDeteccion < finFecha);

                var incidenciasResueltas = await _context.Incidencias
                    .CountAsync(i => i.FechaResolucion.HasValue && i.FechaResolucion >= inicioFecha && i.FechaResolucion < finFecha);

                var incidenciasPendientes = await _context.Incidencias
                    .CountAsync(i => i.EstadoResolucion == "Pendiente" || i.EstadoResolucion == "EnProceso");

                var productosAuditados = await _context.DetallesAuditoria
                    .Include(d => d.Auditoria)
                    .CountAsync(d => d.Auditoria.FechaCreacion >= inicioFecha && d.Auditoria.FechaCreacion < finFecha);

                var evidenciasSubidas = await _context.Evidencias
                    .CountAsync(e => e.FechaCarga >= inicioFecha && e.FechaCarga < finFecha);

                return new ResumenDiarioDTO
                {
                    Fecha = fecha,
                    AuditoriasCreadas = auditoriasCreadas,
                    AuditoriasCerradas = auditoriasCerradas,
                    AuditoriasAbiertas = auditoriasAbiertas,
                    IncidenciasNuevas = incidenciasNuevas,
                    IncidenciasResueltas = incidenciasResueltas,
                    IncidenciasPendientes = incidenciasPendientes,
                    ProductosAuditados = productosAuditados,
                    EvidenciasSubidas = evidenciasSubidas,
                    AlertasImportantes = new List<string>() // TODO: Implementar lógica de alertas
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener resumen diario");
                throw;
            }
        }

        public async Task<List<AlertaDTO>> GetAlertasPendientesAsync(int userId)
        {
            // Implementación delegada a NotificationService
            return new List<AlertaDTO>();
        }

        public async Task<DashboardPersonalizadoDTO> GetDashboardPersonalizadoAsync(int userId, string userRole)
        {
            // TODO: Implementar dashboard personalizado por rol
            return await Task.FromResult(new DashboardPersonalizadoDTO());
        }

        public async Task<ComparativaPeriodosDTO> GetComparativaPeriodosAsync(ComparativaPeriodosRequestDTO request)
        {
            // TODO: Implementar comparativa entre períodos
            return await Task.FromResult(new ComparativaPeriodosDTO());
        }

        public async Task<List<RendimientoOperadorDTO>> GetRendimientoOperadoresAsync(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            // TODO: Implementar métricas de rendimiento de operadores
            return await Task.FromResult(new List<RendimientoOperadorDTO>());
        }

        // Métodos auxiliares
        private decimal CalcularMediana(List<double> valores)
        {
            if (!valores.Any()) return 0;

            var sorted = valores.OrderBy(v => v).ToList();
            int mid = sorted.Count / 2;

            if (sorted.Count % 2 == 0)
                return (decimal)((sorted[mid - 1] + sorted[mid]) / 2);
            else
                return (decimal)sorted[mid];
        }
    }
}
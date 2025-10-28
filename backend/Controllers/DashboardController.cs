using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuditoriaRecepcion.DTOs.Dashboard;
using AuditoriaRecepcion.Services.Interfaces;

namespace AuditoriaRecepcion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener KPIs generales del sistema
        /// </summary>
        [HttpGet("kpis/generales")]
        [ProducesResponseType(typeof(KPIsGeneralesDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<KPIsGeneralesDTO>> GetKPIsGenerales(
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null)
        {
            try
            {
                var kpis = await _dashboardService.GetKPIsGeneralesAsync(fechaDesde, fechaHasta);
                return Ok(kpis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener KPIs generales");
                return StatusCode(500, new { message = "Error al obtener KPIs" });
            }
        }

        /// <summary>
        /// Obtener métricas de auditorías
        /// </summary>
        [HttpGet("metricas/auditorias")]
        [ProducesResponseType(typeof(MetricasAuditoriasDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<MetricasAuditoriasDTO>> GetMetricasAuditorias(
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null,
            [FromQuery] string? agrupacion = "dia") // dia, semana, mes
        {
            try
            {
                var metricas = await _dashboardService.GetMetricasAuditoriasAsync(
                    fechaDesde, 
                    fechaHasta, 
                    agrupacion);
                return Ok(metricas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener métricas de auditorías");
                return StatusCode(500, new { message = "Error al obtener métricas" });
            }
        }

        /// <summary>
        /// Obtener métricas de incidencias
        /// </summary>
        [HttpGet("metricas/incidencias")]
        [ProducesResponseType(typeof(MetricasIncidenciasDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<MetricasIncidenciasDTO>> GetMetricasIncidencias(
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null)
        {
            try
            {
                var metricas = await _dashboardService.GetMetricasIncidenciasAsync(fechaDesde, fechaHasta);
                return Ok(metricas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener métricas de incidencias");
                return StatusCode(500, new { message = "Error al obtener métricas" });
            }
        }

        /// <summary>
        /// Obtener top proveedores con más incidencias
        /// </summary>
        [HttpGet("top/proveedores-incidencias")]
        [Authorize(Roles = "Comprador,JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(List<TopProveedorIncidenciasDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TopProveedorIncidenciasDTO>>> GetTopProveedoresIncidencias(
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null,
            [FromQuery] int top = 10)
        {
            try
            {
                var proveedores = await _dashboardService.GetTopProveedoresIncidenciasAsync(
                    fechaDesde, 
                    fechaHasta, 
                    top);
                return Ok(proveedores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener top proveedores con incidencias");
                return StatusCode(500, new { message = "Error al obtener datos" });
            }
        }

        /// <summary>
        /// Obtener top productos con más incidencias
        /// </summary>
        [HttpGet("top/productos-incidencias")]
        [ProducesResponseType(typeof(List<TopProductoIncidenciasDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TopProductoIncidenciasDTO>>> GetTopProductosIncidencias(
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null,
            [FromQuery] int top = 10)
        {
            try
            {
                var productos = await _dashboardService.GetTopProductosIncidenciasAsync(
                    fechaDesde, 
                    fechaHasta, 
                    top);
                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener top productos con incidencias");
                return StatusCode(500, new { message = "Error al obtener datos" });
            }
        }

        /// <summary>
        /// Obtener distribución de tipos de incidencias
        /// </summary>
        [HttpGet("distribucion/incidencias")]
        [ProducesResponseType(typeof(List<DistribucionIncidenciasDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<DistribucionIncidenciasDTO>>> GetDistribucionIncidencias(
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null)
        {
            try
            {
                var distribucion = await _dashboardService.GetDistribucionIncidenciasAsync(
                    fechaDesde, 
                    fechaHasta);
                return Ok(distribucion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener distribución de incidencias");
                return StatusCode(500, new { message = "Error al obtener distribución" });
            }
        }

        /// <summary>
        /// Obtener tendencia de auditorías y recepciones
        /// </summary>
        [HttpGet("tendencia/auditorias")]
        [ProducesResponseType(typeof(List<TendenciaAuditoriasDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TendenciaAuditoriasDTO>>> GetTendenciaAuditorias(
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null,
            [FromQuery] string agrupacion = "dia") // dia, semana, mes
        {
            try
            {
                var tendencia = await _dashboardService.GetTendenciaAuditoriasAsync(
                    fechaDesde, 
                    fechaHasta, 
                    agrupacion);
                return Ok(tendencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tendencia de auditorías");
                return StatusCode(500, new { message = "Error al obtener tendencia" });
            }
        }

        /// <summary>
        /// Obtener métricas de tiempo de resolución de incidencias
        /// </summary>
        [HttpGet("metricas/tiempo-resolucion")]
        [Authorize(Roles = "JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(MetricasTiempoResolucionDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<MetricasTiempoResolucionDTO>> GetMetricasTiempoResolucion(
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null)
        {
            try
            {
                var metricas = await _dashboardService.GetMetricasTiempoResolucionAsync(
                    fechaDesde, 
                    fechaHasta);
                return Ok(metricas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener métricas de tiempo de resolución");
                return StatusCode(500, new { message = "Error al obtener métricas" });
            }
        }

        /// <summary>
        /// Obtener resumen del día actual
        /// </summary>
        [HttpGet("resumen/hoy")]
        [ProducesResponseType(typeof(ResumenDiarioDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResumenDiarioDTO>> GetResumenHoy()
        {
            try
            {
                var resumen = await _dashboardService.GetResumenDiarioAsync(DateTime.Today);
                return Ok(resumen);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener resumen del día");
                return StatusCode(500, new { message = "Error al obtener resumen" });
            }
        }

        /// <summary>
        /// Obtener alertas y notificaciones pendientes
        /// </summary>
        [HttpGet("alertas/pendientes")]
        [ProducesResponseType(typeof(List<AlertaDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AlertaDTO>>> GetAlertasPendientes()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var alertas = await _dashboardService.GetAlertasPendientesAsync(userId);
                return Ok(alertas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener alertas pendientes");
                return StatusCode(500, new { message = "Error al obtener alertas" });
            }
        }

        /// <summary>
        /// Obtener dashboard personalizado por rol
        /// </summary>
        [HttpGet("personalizado")]
        [ProducesResponseType(typeof(DashboardPersonalizadoDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<DashboardPersonalizadoDTO>> GetDashboardPersonalizado()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var userRole = User.FindFirst("Role")?.Value ?? "";
                
                var dashboard = await _dashboardService.GetDashboardPersonalizadoAsync(userId, userRole);
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener dashboard personalizado");
                return StatusCode(500, new { message = "Error al obtener dashboard" });
            }
        }

        /// <summary>
        /// Obtener comparativa entre períodos
        /// </summary>
        [HttpPost("comparativa")]
        [Authorize(Roles = "JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(ComparativaPeriodosDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<ComparativaPeriodosDTO>> GetComparativaPeriodos(
            [FromBody] ComparativaPeriodosRequestDTO request)
        {
            try
            {
                var comparativa = await _dashboardService.GetComparativaPeriodosAsync(request);
                return Ok(comparativa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener comparativa de períodos");
                return StatusCode(500, new { message = "Error al obtener comparativa" });
            }
        }

        /// <summary>
        /// Obtener métricas de rendimiento de operadores
        /// </summary>
        [HttpGet("rendimiento/operadores")]
        [Authorize(Roles = "JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(List<RendimientoOperadorDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<RendimientoOperadorDTO>>> GetRendimientoOperadores(
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null)
        {
            try
            {
                var rendimiento = await _dashboardService.GetRendimientoOperadoresAsync(
                    fechaDesde, 
                    fechaHasta);
                return Ok(rendimiento);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rendimiento de operadores");
                return StatusCode(500, new { message = "Error al obtener rendimiento" });
            }
        }
    }
}
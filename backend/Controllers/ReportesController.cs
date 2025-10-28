using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuditoriaRecepcion.DTOs;
using AuditoriaRecepcion.Services.Interfaces;

namespace AuditoriaRecepcion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteService _reporteService;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(IReporteService reporteService, ILogger<ReportesController> logger)
        {
            _reporteService = reporteService;
            _logger = logger;
        }

        /// <summary>
        /// Generar reporte de auditoría en PDF
        /// </summary>
        [HttpGet("auditoria/{auditoriaId}/pdf")]
        [Authorize(Roles = "Operador,JefeLogistica,Comprador,Administrador")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GenerarReporteAuditoriaPDF(int auditoriaId)
        {
            try
            {
                var fileBytes = await _reporteService.GenerarReporteAuditoriaPDFAsync(auditoriaId);
                
                if (fileBytes == null)
                    return NotFound(new { message = "Auditoría no encontrada" });

                return File(fileBytes, 
                    "application/pdf", 
                    $"Auditoria_{auditoriaId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte PDF de auditoría {Id}", auditoriaId);
                return StatusCode(500, new { message = "Error al generar reporte PDF" });
            }
        }

        /// <summary>
        /// Generar reporte de auditoría en Excel
        /// </summary>
        [HttpGet("auditoria/{auditoriaId}/excel")]
        [Authorize(Roles = "Operador,JefeLogistica,Comprador,Administrador")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GenerarReporteAuditoriaExcel(int auditoriaId)
        {
            try
            {
                var fileBytes = await _reporteService.GenerarReporteAuditoriaExcelAsync(auditoriaId);
                
                if (fileBytes == null)
                    return NotFound(new { message = "Auditoría no encontrada" });

                return File(fileBytes, 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    $"Auditoria_{auditoriaId}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte Excel de auditoría {Id}", auditoriaId);
                return StatusCode(500, new { message = "Error al generar reporte Excel" });
            }
        }

        /// <summary>
        /// Generar reporte consolidado de auditorías en Excel
        /// </summary>
        [HttpPost("auditorias/consolidado")]
        [Authorize(Roles = "JefeLogistica,Comprador,Administrador")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerarReporteConsolidadoExcel([FromBody] ReporteConsolidadoRequestDTO request)
        {
            try
            {
                var fileBytes = await _reporteService.GenerarReporteConsolidadoExcelAsync(request);
                
                return File(fileBytes, 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    $"Reporte_Consolidado_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte consolidado");
                return StatusCode(500, new { message = "Error al generar reporte consolidado" });
            }
        }

        /// <summary>
        /// Generar reporte de incidencias en Excel
        /// </summary>
        [HttpPost("incidencias/excel")]
        [Authorize(Roles = "JefeLogistica,Comprador,Administrador")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerarReporteIncidenciasExcel([FromBody] ReporteIncidenciasRequestDTO request)
        {
            try
            {
                var fileBytes = await _reporteService.GenerarReporteIncidenciasExcelAsync(request);
                
                return File(fileBytes, 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    $"Reporte_Incidencias_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de incidencias");
                return StatusCode(500, new { message = "Error al generar reporte de incidencias" });
            }
        }

        /// <summary>
        /// Generar reporte de desempeño de proveedores en Excel
        /// </summary>
        [HttpPost("proveedores/desempeno")]
        [Authorize(Roles = "Comprador,JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerarReporteDesempenoProveedores([FromBody] ReporteProveedoresRequestDTO request)
        {
            try
            {
                var fileBytes = await _reporteService.GenerarReporteDesempenoProveedoresAsync(request);
                
                return File(fileBytes, 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    $"Reporte_Proveedores_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de proveedores");
                return StatusCode(500, new { message = "Error al generar reporte de proveedores" });
            }
        }

        /// <summary>
        /// Generar reporte de KPIs en PDF
        /// </summary>
        [HttpPost("kpis/pdf")]
        [Authorize(Roles = "JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerarReporteKPIsPDF([FromBody] ReporteKPIsRequestDTO request)
        {
            try
            {
                var fileBytes = await _reporteService.GenerarReporteKPIsPDFAsync(request);
                
                return File(fileBytes, 
                    "application/pdf", 
                    $"Reporte_KPIs_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de KPIs");
                return StatusCode(500, new { message = "Error al generar reporte de KPIs" });
            }
        }

        /// <summary>
        /// Generar reporte personalizado
        /// </summary>
        [HttpPost("personalizado")]
        [Authorize(Roles = "JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerarReportePersonalizado([FromBody] ReportePersonalizadoRequestDTO request)
        {
            try
            {
                var fileBytes = await _reporteService.GenerarReportePersonalizadoAsync(request);
                
                var contentType = request.Formato.ToLower() == "pdf" 
                    ? "application/pdf" 
                    : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                
                var extension = request.Formato.ToLower() == "pdf" ? "pdf" : "xlsx";
                
                return File(fileBytes, 
                    contentType, 
                    $"Reporte_Personalizado_{DateTime.Now:yyyyMMdd_HHmmss}.{extension}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte personalizado");
                return StatusCode(500, new { message = "Error al generar reporte personalizado" });
            }
        }

        /// <summary>
        /// Obtener historial de reportes generados
        /// </summary>
        [HttpGet("historial")]
        [Authorize(Roles = "JefeLogistica,Comprador,Administrador")]
        [ProducesResponseType(typeof(PaginatedResult<ReporteHistorialDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResult<ReporteHistorialDTO>>> GetHistorialReportes(
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null,
            [FromQuery] string? tipoReporte = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var filtro = new ReporteHistorialFiltroDTO
                {
                    FechaDesde = fechaDesde,
                    FechaHasta = fechaHasta,
                    TipoReporte = tipoReporte,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _reporteService.GetHistorialReportesAsync(filtro);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial de reportes");
                return StatusCode(500, new { message = "Error al obtener historial" });
            }
        }

        /// <summary>
        /// Programar generación automática de reporte
        /// </summary>
        [HttpPost("programar")]
        [Authorize(Roles = "JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(ReporteProgramadoDTO), StatusCodes.Status201Created)]
        public async Task<ActionResult<ReporteProgramadoDTO>> ProgramarReporte([FromBody] ProgramarReporteDTO dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var reporte = await _reporteService.ProgramarReporteAsync(dto, userId);
                
                return CreatedAtAction(
                    nameof(GetReporteProgramado), 
                    new { id = reporte.ReporteProgramadoID }, 
                    reporte);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al programar reporte");
                return StatusCode(500, new { message = "Error al programar reporte" });
            }
        }

        /// <summary>
        /// Obtener reporte programado por ID
        /// </summary>
        [HttpGet("programado/{id}")]
        [Authorize(Roles = "JefeLogistica,Administrador")]
        [ProducesResponseType(typeof(ReporteProgramadoDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<ReporteProgramadoDTO>> GetReporteProgramado(int id)
        {
            try
            {
                var reporte = await _reporteService.GetReporteProgramadoByIdAsync(id);
                if (reporte == null)
                    return NotFound(new { message = "Reporte programado no encontrado" });

                return Ok(reporte);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte programado {Id}", id);
                return StatusCode(500, new { message = "Error al obtener reporte programado" });
            }
        }

        /// <summary>
        /// Cancelar reporte programado
        /// </summary>
        [HttpDelete("programado/{id}")]
        [Authorize(Roles = "JefeLogistica,Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CancelarReporteProgramado(int id)
        {
            try
            {
                await _reporteService.CancelarReporteProgramadoAsync(id);
                return Ok(new { message = "Reporte programado cancelado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar reporte programado {Id}", id);
                return StatusCode(500, new { message = "Error al cancelar reporte" });
            }
        }
    }
}
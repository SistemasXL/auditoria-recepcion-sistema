using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuditoriaRecepcion.DTOs;
using AuditoriaRecepcion.Services.Interfaces;

namespace AuditoriaRecepcion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EvidenciasController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<EvidenciasController> _logger;

        public EvidenciasController(IFileStorageService fileStorageService, ILogger<EvidenciasController> logger)
        {
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        /// <summary>
        /// Subir evidencia (foto o video)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Operador,JefeLogistica,Administrador")]
        [RequestSizeLimit(52428800)] // 50 MB
        [ProducesResponseType(typeof(EvidenciaDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EvidenciaDTO>> UploadEvidencia(
            [FromForm] int auditoriaId,
            [FromForm] int? detalleAuditoriaId,
            [FromForm] int? incidenciaId,
            [FromForm] string tipoEvidencia,
            [FromForm] string? descripcion,
            [FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "Archivo no válido" });

                // Validar tipo de archivo
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".mp4", ".mov", ".avi" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(extension))
                    return BadRequest(new { message = "Tipo de archivo no permitido" });

                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

                var dto = new CreateEvidenciaDTO
                {
                    AuditoriaID = auditoriaId,
                    DetalleAuditoriaID = detalleAuditoriaId,
                    IncidenciaID = incidenciaId,
                    TipoEvidencia = tipoEvidencia,
                    Descripcion = descripcion
                };

                var evidencia = await _fileStorageService.SaveEvidenciaAsync(file, dto, userId);
                
                return CreatedAtAction(
                    nameof(GetEvidencia), 
                    new { id = evidencia.EvidenciaID }, 
                    evidencia);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir evidencia");
                return StatusCode(500, new { message = "Error al subir evidencia" });
            }
        }

        /// <summary>
        /// Subir múltiples evidencias
        /// </summary>
        [HttpPost("multiple")]
        [Authorize(Roles = "Operador,JefeLogistica,Administrador")]
        [RequestSizeLimit(104857600)] // 100 MB
        [ProducesResponseType(typeof(List<EvidenciaDTO>), StatusCodes.Status201Created)]
        public async Task<ActionResult<List<EvidenciaDTO>>> UploadMultipleEvidencias(
            [FromForm] int auditoriaId,
            [FromForm] int? detalleAuditoriaId,
            [FromForm] int? incidenciaId,
            [FromForm] string tipoEvidencia,
            [FromForm] List<IFormFile> files)
        {
            try
            {
                if (files == null || !files.Any())
                    return BadRequest(new { message = "No se recibieron archivos" });

                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var evidencias = new List<EvidenciaDTO>();

                foreach (var file in files)
                {
                    var dto = new CreateEvidenciaDTO
                    {
                        AuditoriaID = auditoriaId,
                        DetalleAuditoriaID = detalleAuditoriaId,
                        IncidenciaID = incidenciaId,
                        TipoEvidencia = tipoEvidencia
                    };

                    var evidencia = await _fileStorageService.SaveEvidenciaAsync(file, dto, userId);
                    evidencias.Add(evidencia);
                }

                return CreatedAtAction(nameof(GetEvidenciasByAuditoria), new { auditoriaId }, evidencias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir evidencias múltiples");
                return StatusCode(500, new { message = "Error al subir evidencias" });
            }
        }

        /// <summary>
        /// Obtener evidencia por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EvidenciaDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EvidenciaDTO>> GetEvidencia(int id)
        {
            try
            {
                var evidencia = await _fileStorageService.GetEvidenciaByIdAsync(id);
                if (evidencia == null)
                    return NotFound(new { message = "Evidencia no encontrada" });

                return Ok(evidencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener evidencia {Id}", id);
                return StatusCode(500, new { message = "Error al obtener evidencia" });
            }
        }

        /// <summary>
        /// Descargar archivo de evidencia
        /// </summary>
        [HttpGet("{id}/download")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadEvidencia(int id)
        {
            try
            {
                var file = await _fileStorageService.GetEvidenciaFileAsync(id);
                if (file == null)
                    return NotFound(new { message = "Evidencia no encontrada" });

                return File(file.FileBytes, file.ContentType, file.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al descargar evidencia {Id}", id);
                return StatusCode(500, new { message = "Error al descargar evidencia" });
            }
        }

        /// <summary>
        /// Obtener evidencias por auditoría
        /// </summary>
        [HttpGet("auditoria/{auditoriaId}")]
        [ProducesResponseType(typeof(List<EvidenciaDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<EvidenciaDTO>>> GetEvidenciasByAuditoria(int auditoriaId)
        {
            try
            {
                var evidencias = await _fileStorageService.GetEvidenciasByAuditoriaAsync(auditoriaId);
                return Ok(evidencias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener evidencias de auditoría {Id}", auditoriaId);
                return StatusCode(500, new { message = "Error al obtener evidencias" });
            }
        }

        /// <summary>
        /// Obtener evidencias por incidencia
        /// </summary>
        [HttpGet("incidencia/{incidenciaId}")]
        [ProducesResponseType(typeof(List<EvidenciaDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<EvidenciaDTO>>> GetEvidenciasByIncidencia(int incidenciaId)
        {
            try
            {
                var evidencias = await _fileStorageService.GetEvidenciasByIncidenciaAsync(incidenciaId);
                return Ok(evidencias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener evidencias de incidencia {Id}", incidenciaId);
                return StatusCode(500, new { message = "Error al obtener evidencias" });
            }
        }

        /// <summary>
        /// Eliminar evidencia
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "JefeLogistica,Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteEvidencia(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                await _fileStorageService.DeleteEvidenciaAsync(id, userId);
                return Ok(new { message = "Evidencia eliminada correctamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar evidencia {Id}", id);
                return StatusCode(500, new { message = "Error al eliminar evidencia" });
            }
        }
    }
}
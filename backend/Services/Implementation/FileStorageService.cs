using AuditoriaRecepcion.Data;
using AuditoriaRecepcion.DTOs.Evidencia;
using AuditoriaRecepcion.Models;
using AuditoriaRecepcion.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AuditoriaRecepcion.Services.Implementation
{
    public class FileStorageService : IFileStorageService
    {
        private readonly AuditoriaRecepcionContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileStorageService> _logger;
        private readonly string _storagePath;

        public FileStorageService(
            AuditoriaRecepcionContext context,
            IConfiguration configuration,
            ILogger<FileStorageService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _storagePath = configuration["FileStorage:Path"] ?? Path.Combine(Directory.GetCurrentDirectory(), "Storage");

            // Crear directorio de almacenamiento si no existe
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        public async Task<EvidenciaDTO> SaveEvidenciaAsync(IFormFile file, CreateEvidenciaDTO dto, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validar archivo
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".mov", ".avi", ".pdf" };
                var maxSizeBytes = 52428800; // 50 MB

                if (!await ValidateFileAsync(file, allowedExtensions, maxSizeBytes))
                    throw new InvalidOperationException("Archivo no válido o excede el tamaño máximo permitido");

                // Validar auditoría
                var auditoria = await _context.AuditoriasRecepcion.FindAsync(dto.AuditoriaID);
                if (auditoria == null)
                    throw new InvalidOperationException("Auditoría no encontrada");

                // Generar nombre único para el archivo
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

                // Crear estructura de carpetas: Storage/Auditorias/{AuditoriaID}/Evidencias/
                var auditoriaFolder = Path.Combine(_storagePath, "Auditorias", dto.AuditoriaID.ToString(), "Evidencias");
                if (!Directory.Exists(auditoriaFolder))
                {
                    Directory.CreateDirectory(auditoriaFolder);
                }

                var filePath = Path.Combine(auditoriaFolder, uniqueFileName);

                // Guardar archivo físicamente
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Crear registro en BD
                var evidencia = new Evidencia
                {
                    AuditoriaID = dto.AuditoriaID,
                    DetalleAuditoriaID = dto.DetalleAuditoriaID,
                    TipoArchivo = file.ContentType,
                    NombreArchivo = file.FileName,
                    RutaArchivo = filePath,
                    TamañoKB = (int?)(file.Length / 1024),
                    FechaSubida = DateTime.Now
                };

                _context.Evidencias.Add(evidencia);
                await _context.SaveChangesAsync();

                // Registrar auditoría
                await RegistrarAuditoriaAccionAsync(userId, "SubirEvidencia", "Evidencia", evidencia.EvidenciaID);

                await transaction.CommitAsync();

                _logger.LogInformation("Evidencia guardada: {Id} para auditoría {AuditoriaId} por usuario {UserId}",
                    evidencia.EvidenciaID, dto.AuditoriaID, userId);

                return MapToEvidenciaDTO(evidencia);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al guardar evidencia");
                throw;
            }
        }

        public async Task<EvidenciaDTO> GetEvidenciaByIdAsync(int id)
        {
            try
            {
                var evidencia = await _context.Evidencias
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.EvidenciaID == id);

                return evidencia != null ? MapToEvidenciaDTO(evidencia) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener evidencia {Id}", id);
                throw;
            }
        }

        public async Task<FileDownloadDTO> GetEvidenciaFileAsync(int id)
        {
            try
            {
                var evidencia = await _context.Evidencias
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.EvidenciaID == id);

                if (evidencia == null)
                    return null;

                if (!File.Exists(evidencia.RutaArchivo))
                {
                    _logger.LogWarning("Archivo físico no encontrado: {RutaArchivo}", evidencia.RutaArchivo);
                    throw new FileNotFoundException("Archivo no encontrado en el servidor");
                }

                var fileBytes = await File.ReadAllBytesAsync(evidencia.RutaArchivo);

                return new FileDownloadDTO
                {
                    FileBytes = fileBytes,
                    ContentType = evidencia.TipoArchivo,
                    FileName = evidencia.NombreArchivo
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener archivo de evidencia {Id}", id);
                throw;
            }
        }

        public async Task<List<EvidenciaDTO>> GetEvidenciasByAuditoriaAsync(int auditoriaId)
        {
            try
            {
                var evidencias = await _context.Evidencias
                    .Where(e => e.AuditoriaID == auditoriaId)
                    .OrderBy(e => e.FechaSubida)
                    .AsNoTracking()
                    .ToListAsync();

                return evidencias.Select(e => MapToEvidenciaDTO(e)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener evidencias de auditoría {AuditoriaId}", auditoriaId);
                throw;
            }
        }

        public async Task<List<EvidenciaDTO>> GetEvidenciasByIncidenciaAsync(int incidenciaId)
        {
            try
            {
                // IncidenciaID no existe en el modelo Evidencia
                // Devolver lista vacía por ahora
                return new List<EvidenciaDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener evidencias de incidencia {IncidenciaId}", incidenciaId);
                throw;
            }
        }

        public async Task DeleteEvidenciaAsync(int id, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var evidencia = await _context.Evidencias.FindAsync(id);
                if (evidencia == null)
                    throw new InvalidOperationException("Evidencia no encontrada");

                var rutaArchivo = evidencia.RutaArchivo;

                _context.Evidencias.Remove(evidencia);
                await _context.SaveChangesAsync();

                // Eliminar archivo físico
                if (File.Exists(rutaArchivo))
                {
                    File.Delete(rutaArchivo);
                }

                // Registrar auditoría
                await RegistrarAuditoriaAccionAsync(userId, "EliminarEvidencia", "Evidencia", id);

                await transaction.CommitAsync();

                _logger.LogInformation("Evidencia eliminada: {Id} por usuario {UserId}", id, userId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al eliminar evidencia {Id}", id);
                throw;
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            try
            {
                var folderPath = Path.Combine(_storagePath, folder);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(folderPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar archivo en carpeta {Folder}", folder);
                throw;
            }
        }

        public async Task DeleteFileAsync(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar archivo {FilePath}", filePath);
                throw;
            }
        }

        public async Task<bool> ValidateFileAsync(IFormFile file, string[] allowedExtensions, long maxSizeBytes)
        {
            if (file == null || file.Length == 0)
                return false;

            if (file.Length > maxSizeBytes)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return false;

            return await Task.FromResult(true);
        }

        // Métodos privados auxiliares
        private async Task RegistrarAuditoriaAccionAsync(int userId, string accion, string tabla, int registroId)
        {
            try
            {
                var log = new AuditoriaLog
                {
                    UsuarioID = userId,
                    TipoAccion = accion,
                    TablaAfectada = tabla,
                    RegistroID = registroId,
                    FechaHora = DateTime.Now
                };

                _context.AuditoriaLogs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar auditoría de acción");
                // No lanzar excepción para no afectar el flujo principal
            }
        }

        private EvidenciaDTO MapToEvidenciaDTO(Evidencia evidencia)
        {
            return new EvidenciaDTO
            {
                EvidenciaID = evidencia.EvidenciaID,
                AuditoriaID = evidencia.AuditoriaID,
                DetalleAuditoriaID = evidencia.DetalleAuditoriaID,
                TipoEvidencia = evidencia.TipoArchivo,
                NombreArchivo = evidencia.NombreArchivo,
                RutaArchivo = evidencia.RutaArchivo,
                TipoArchivo = evidencia.TipoArchivo,
                TamanoBytes = evidencia.TamañoKB ?? 0,
                TamanoLegible = FormatBytes(evidencia.TamañoKB ?? 0),
                FechaCarga = evidencia.FechaSubida,
                UrlDescarga = $"/api/evidencias/{evidencia.EvidenciaID}/download",
                UrlVisualizacion = $"/api/evidencias/{evidencia.EvidenciaID}"
            };
        }

        private string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
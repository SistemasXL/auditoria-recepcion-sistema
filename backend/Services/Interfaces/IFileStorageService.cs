using AuditoriaRecepcion.DTOs.Evidencia;
using Microsoft.AspNetCore.Http;

namespace AuditoriaRecepcion.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<EvidenciaDTO> SaveEvidenciaAsync(IFormFile file, CreateEvidenciaDTO dto, int userId);
        Task<EvidenciaDTO> GetEvidenciaByIdAsync(int id);
        Task<FileDownloadDTO> GetEvidenciaFileAsync(int id);
        Task<List<EvidenciaDTO>> GetEvidenciasByAuditoriaAsync(int auditoriaId);
        Task<List<EvidenciaDTO>> GetEvidenciasByIncidenciaAsync(int incidenciaId);
        Task DeleteEvidenciaAsync(int id, int userId);
        Task<string> SaveFileAsync(IFormFile file, string folder);
        Task DeleteFileAsync(string filePath);
        Task<bool> ValidateFileAsync(IFormFile file, string[] allowedExtensions, long maxSizeBytes);
    }
}
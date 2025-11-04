using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TextileCRM.Domain.Entities;

namespace TextileCRM.Application.Interfaces
{
    public interface IFileService
    {
        Task<IEnumerable<FileAttachment>> GetAllFilesAsync();
        Task<FileAttachment?> GetFileByIdAsync(int id);
        Task<IEnumerable<FileAttachment>> GetFilesByEntityAsync(string entityType, int entityId);
        Task<IEnumerable<FileAttachment>> GetFilesByCategoryAsync(FileCategory category);
        Task<FileAttachment> UploadFileAsync(Stream fileStream, string fileName, string contentType, FileCategory category, string entityType, int entityId, int uploadedBy);
        Task<(Stream stream, string contentType, string fileName)> DownloadFileAsync(int id);
        Task DeleteFileAsync(int id);
        Task<bool> FileExistsAsync(int id);
        Task<long> GetTotalFileSizeAsync();
    }
}


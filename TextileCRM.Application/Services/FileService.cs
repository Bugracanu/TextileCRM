using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.Application.Services
{
    public class FileService : IFileService
    {
        private readonly IRepository<FileAttachment> _fileRepository;
        private readonly string _uploadPath;

        public FileService(IRepository<FileAttachment> fileRepository)
        {
            _fileRepository = fileRepository;
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            
            // Ensure upload directory exists
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<IEnumerable<FileAttachment>> GetAllFilesAsync()
        {
            return await _fileRepository.GetAllAsync();
        }

        public async Task<FileAttachment?> GetFileByIdAsync(int id)
        {
            return await _fileRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<FileAttachment>> GetFilesByEntityAsync(string entityType, int entityId)
        {
            var files = await _fileRepository.GetAllAsync();
            return files.Where(f => f.EntityType == entityType && f.EntityId == entityId);
        }

        public async Task<IEnumerable<FileAttachment>> GetFilesByCategoryAsync(FileCategory category)
        {
            var files = await _fileRepository.GetAllAsync();
            return files.Where(f => f.Category == category);
        }

        public async Task<FileAttachment> UploadFileAsync(Stream fileStream, string fileName, string contentType, 
            FileCategory category, string entityType, int entityId, int uploadedBy)
        {
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_uploadPath, uniqueFileName);

            // Save file to disk
            using (var outputStream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(outputStream);
            }

            var fileSize = new FileInfo(filePath).Length;

            var fileAttachment = new FileAttachment
            {
                FileName = uniqueFileName,
                OriginalFileName = fileName,
                FilePath = filePath,
                FileExtension = fileExtension,
                FileSize = fileSize,
                ContentType = contentType,
                Category = category,
                EntityType = entityType,
                EntityId = entityId,
                UploadedBy = uploadedBy,
                UploadedDate = DateTime.Now
            };

            await _fileRepository.AddAsync(fileAttachment);
            return fileAttachment;
        }

        public async Task<(Stream stream, string contentType, string fileName)> DownloadFileAsync(int id)
        {
            var file = await _fileRepository.GetByIdAsync(id);
            if (file == null || !File.Exists(file.FilePath))
            {
                throw new FileNotFoundException("File not found");
            }

            var stream = new FileStream(file.FilePath, FileMode.Open, FileAccess.Read);
            return (stream, file.ContentType, file.OriginalFileName);
        }

        public async Task DeleteFileAsync(int id)
        {
            var file = await _fileRepository.GetByIdAsync(id);
            if (file != null)
            {
                // Delete physical file
                if (File.Exists(file.FilePath))
                {
                    File.Delete(file.FilePath);
                }

                // Delete database record
                await _fileRepository.DeleteAsync(id);
            }
        }

        public async Task<bool> FileExistsAsync(int id)
        {
            var file = await _fileRepository.GetByIdAsync(id);
            return file != null && File.Exists(file.FilePath);
        }

        public async Task<long> GetTotalFileSizeAsync()
        {
            var files = await _fileRepository.GetAllAsync();
            return files.Sum(f => f.FileSize);
        }
    }
}


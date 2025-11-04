using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;
using System.Security.Claims;

namespace TextileCRM.WebUI.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class FilesApiController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesApiController(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Tüm dosyaları listeler
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FileAttachment>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FileAttachment>>> GetAll()
        {
            var files = await _fileService.GetAllFilesAsync();
            return Ok(files);
        }

        /// <summary>
        /// ID'ye göre dosya bilgisi getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FileAttachment), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FileAttachment>> GetById(int id)
        {
            var file = await _fileService.GetFileByIdAsync(id);
            if (file == null)
            {
                return NotFound(new { message = "Dosya bulunamadı" });
            }
            return Ok(file);
        }

        /// <summary>
        /// Entity'ye ait dosyaları listeler (Order, Invoice, Product vs.)
        /// </summary>
        [HttpGet("entity/{entityType}/{entityId}")]
        [ProducesResponseType(typeof(IEnumerable<FileAttachment>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FileAttachment>>> GetByEntity(string entityType, int entityId)
        {
            var files = await _fileService.GetFilesByEntityAsync(entityType, entityId);
            return Ok(files);
        }

        /// <summary>
        /// Kategoriye göre dosyaları filtreler
        /// </summary>
        [HttpGet("category/{category}")]
        [ProducesResponseType(typeof(IEnumerable<FileAttachment>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FileAttachment>>> GetByCategory(FileCategory category)
        {
            var files = await _fileService.GetFilesByCategoryAsync(category);
            return Ok(files);
        }

        /// <summary>
        /// Dosya yükler
        /// </summary>
        [HttpPost("upload")]
        [ProducesResponseType(typeof(FileAttachment), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<FileAttachment>> Upload(
            [FromForm] IFormFile file,
            [FromForm] FileCategory category,
            [FromForm] string entityType,
            [FromForm] int entityId,
            [FromForm] string? description)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Geçerli bir dosya seçiniz" });
            }

            // Get current user ID
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int uploadedBy = int.TryParse(userIdClaim, out int userId) ? userId : 1;

            using var stream = file.OpenReadStream();
            var uploadedFile = await _fileService.UploadFileAsync(
                stream,
                file.FileName,
                file.ContentType,
                category,
                entityType,
                entityId,
                uploadedBy
            );

            if (!string.IsNullOrEmpty(description))
            {
                uploadedFile.Description = description;
            }

            return CreatedAtAction(nameof(GetById), new { id = uploadedFile.Id }, uploadedFile);
        }

        /// <summary>
        /// Dosyayı indirir
        /// </summary>
        [HttpGet("{id}/download")]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Download(int id)
        {
            try
            {
                var (stream, contentType, fileName) = await _fileService.DownloadFileAsync(id);
                return File(stream, contentType, fileName);
            }
            catch (FileNotFoundException)
            {
                return NotFound(new { message = "Dosya bulunamadı" });
            }
        }

        /// <summary>
        /// Dosyayı siler
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var file = await _fileService.GetFileByIdAsync(id);
            if (file == null)
            {
                return NotFound(new { message = "Dosya bulunamadı" });
            }

            await _fileService.DeleteFileAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Toplam dosya boyutunu getirir
        /// </summary>
        [HttpGet("total-size")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetTotalSize()
        {
            var totalSize = await _fileService.GetTotalFileSizeAsync();
            var sizeInMB = totalSize / (1024.0 * 1024.0);
            var sizeInGB = totalSize / (1024.0 * 1024.0 * 1024.0);

            return Ok(new
            {
                totalBytes = totalSize,
                totalMB = Math.Round(sizeInMB, 2),
                totalGB = Math.Round(sizeInGB, 2)
            });
        }
    }
}


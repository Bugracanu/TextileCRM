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
    public class NotificationsApiController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsApiController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Kullanıcıya ait tüm bildirimleri listeler
        /// </summary>
        [HttpGet("my-notifications")]
        [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Notification>>> GetMyNotifications()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.TryParse(userIdClaim, out int id) ? id : 0;

            var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);
            return Ok(notifications);
        }

        /// <summary>
        /// Kullanıcıya ait okunmamış bildirimleri listeler
        /// </summary>
        [HttpGet("my-notifications/unread")]
        [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Notification>>> GetMyUnreadNotifications()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.TryParse(userIdClaim, out int id) ? id : 0;

            var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);
            return Ok(notifications);
        }

        /// <summary>
        /// Okunmamış bildirim sayısını getirir
        /// </summary>
        [HttpGet("my-notifications/unread-count")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetUnreadCount()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.TryParse(userIdClaim, out int id) ? id : 0;

            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(new { unreadCount = count });
        }

        /// <summary>
        /// Tüm bildirimleri listeler (Admin)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Notification>>> GetAll()
        {
            var notifications = await _notificationService.GetAllNotificationsAsync();
            return Ok(notifications);
        }

        /// <summary>
        /// ID'ye göre bildirim getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Notification>> GetById(int id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
            {
                return NotFound(new { message = "Bildirim bulunamadı" });
            }
            return Ok(notification);
        }

        /// <summary>
        /// Yeni bildirim oluşturur
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Notification>> Create([FromBody] Notification notification)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdNotification = await _notificationService.CreateNotificationAsync(notification);
            return CreatedAtAction(nameof(GetById), new { id = createdNotification.Id }, createdNotification);
        }

        /// <summary>
        /// Birden fazla kullanıcıya bildirim gönderir
        /// </summary>
        [HttpPost("broadcast")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BroadcastNotification([FromBody] BroadcastNotificationRequest request)
        {
            if (!ModelState.IsValid || request.UserIds == null || !request.UserIds.Any())
            {
                return BadRequest(new { message = "Geçersiz istek" });
            }

            await _notificationService.CreateNotificationForUsersAsync(
                request.UserIds,
                request.Title,
                request.Message,
                request.Type,
                request.Priority
            );

            return Ok(new { message = "Bildirimler başarıyla gönderildi" });
        }

        /// <summary>
        /// Bildirimi okundu olarak işaretler
        /// </summary>
        [HttpPatch("{id}/mark-as-read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
            {
                return NotFound(new { message = "Bildirim bulunamadı" });
            }

            await _notificationService.MarkAsReadAsync(id);
            return Ok(new { message = "Bildirim okundu olarak işaretlendi" });
        }

        /// <summary>
        /// Tüm bildirimleri okundu olarak işaretler
        /// </summary>
        [HttpPost("mark-all-as-read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.TryParse(userIdClaim, out int id) ? id : 0;

            await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(new { message = "Tüm bildirimler okundu olarak işaretlendi" });
        }

        /// <summary>
        /// Bildirimi siler
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
            {
                return NotFound(new { message = "Bildirim bulunamadı" });
            }

            await _notificationService.DeleteNotificationAsync(id);
            return NoContent();
        }
    }

    public class BroadcastNotificationRequest
    {
        public IEnumerable<int> UserIds { get; set; } = new List<int>();
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
    }
}


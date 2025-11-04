using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Notification> _notificationRepository;

        public NotificationService(IRepository<Notification> notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
        {
            return await _notificationRepository.GetAllAsync();
        }

        public async Task<Notification?> GetNotificationByIdAsync(int id)
        {
            return await _notificationRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(int userId)
        {
            var notifications = await _notificationRepository.GetAllAsync();
            return notifications.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedDate);
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(int userId)
        {
            var notifications = await _notificationRepository.GetAllAsync();
            return notifications.Where(n => n.UserId == userId && !n.IsRead).OrderByDescending(n => n.CreatedDate);
        }

        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            notification.CreatedDate = DateTime.Now;
            notification.IsRead = false;
            await _notificationRepository.AddAsync(notification);
            return notification;
        }

        public async Task CreateNotificationForUsersAsync(IEnumerable<int> userIds, string title, string message, 
            NotificationType type, NotificationPriority priority = NotificationPriority.Normal)
        {
            foreach (var userId in userIds)
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    Type = type,
                    Priority = priority,
                    IsRead = false,
                    CreatedDate = DateTime.Now
                };
                await _notificationRepository.AddAsync(notification);
            }
        }

        public async Task UpdateNotificationAsync(Notification notification)
        {
            await _notificationRepository.UpdateAsync(notification);
        }

        public async Task DeleteNotificationAsync(int id)
        {
            await _notificationRepository.DeleteAsync(id);
        }

        public async Task MarkAsReadAsync(int id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadDate = DateTime.Now;
                await _notificationRepository.UpdateAsync(notification);
            }
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            var notifications = await GetUnreadNotificationsAsync(userId);
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadDate = DateTime.Now;
                await _notificationRepository.UpdateAsync(notification);
            }
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            var unread = await GetUnreadNotificationsAsync(userId);
            return unread.Count();
        }
    }
}


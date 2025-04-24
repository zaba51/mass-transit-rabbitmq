using NotificationAPI.DbContexts;
using NotificationAPI.Entities;
using Microsoft.EntityFrameworkCore;
using MassTransit;

namespace NotificationAPI.Repositories
{
    public class NotificationRepository: INotificationRepository
    {
        private readonly NotificationDbContext _context;

        public NotificationRepository(NotificationDbContext context)
        {

            _context = context;
        }

        public async Task<IEnumerable<Notification>> GetAllAsync()
        {
            return await _context.Notifications.ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(Guid id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        public async Task<Notification> AddAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }
        public async Task<List<Notification>> GetPendingNotificationsAsync(DateTime nowUtc)
        {
            return await _context.Notifications
                .Where(n =>
                    n.Status == NotificationStatus.Scheduled &&
                    (n.ScheduledAtUtc <= nowUtc || n.ForceSend)
                )
                .OrderByDescending(n => n.Priority)
                .ThenBy(n => n.ScheduledAtUtc)
                .ToListAsync();
        }
        public async Task<Notification?> UpdateAsync(Notification notification)
        {
            var existingnotification = await _context.Notifications.FindAsync(notification.Id);
            if (existingnotification == null) return null;

            existingnotification.SentAt = notification.SentAt;

            await _context.SaveChangesAsync();
            return existingnotification;
        }

        public async Task RemoveAsync(Notification notification)
        {
            var existingnotification = await _context.Notifications.FindAsync(notification.Id);
            if (existingnotification == null) return;
            _context.Notifications.Remove(notification);

            await _context.SaveChangesAsync();
        }
    }
}

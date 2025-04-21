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

        public async Task<Notification> AddAsync(Notification product)
        {
            _context.Notifications.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }
        public async Task<List<Notification>> GetPendingNotificationsAsync(DateTime nowUtc)
        {
            return await _context.Notifications
                .Where(n =>
                    n.Status == NotificationStatus.Scheduled &&
                    !n.ForceSend && n.ScheduledAtUtc <= nowUtc ||
                    n.ForceSend
                )
                .Where(n => n.RetryCount < 3)
                .OrderByDescending(n => n.Priority)
                .ThenBy(n => n.ScheduledAtUtc)
                .ToListAsync();
        }
        public async Task<Notification?> UpdateAsync(Notification product)
        {
            var existingProduct = await _context.Notifications.FindAsync(product.Id);
            if (existingProduct == null) return null;

            existingProduct.SentAt = product.SentAt;
            existingProduct.RetryCount = product.RetryCount;

            await _context.SaveChangesAsync();
            return existingProduct;
        }

    }
}

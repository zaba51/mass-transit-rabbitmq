using NotificationAPI.Entities;

namespace NotificationAPI.Repositories
{
    public interface INotificationRepository
    {
        public Task<IEnumerable<Notification>> GetAllAsync();
        public Task<Notification?> GetByIdAsync(Guid Id);
        public Task<Notification> AddAsync(Notification product);
        public Task<List<Notification>> GetPendingNotificationsAsync(DateTime nowUtc);
        public Task<Notification?> UpdateAsync(Notification product);
    }
}

using NotificationAPI.Entities;

namespace NotificationAPI.Repositories
{
    public interface INotificationRepository
    {
        public Task<IEnumerable<Notification>> GetAllAsync();
        public Task<Notification?> GetByIdAsync(Guid Id);
        public Task<Notification> AddAsync(Notification notification);
        public Task<List<Notification>> GetPendingNotificationsAsync(DateTime nowUtc);
        public Task RemoveAsync(Notification notification);
        public Task<Notification?> UpdateAsync(Notification notification);
    }
}

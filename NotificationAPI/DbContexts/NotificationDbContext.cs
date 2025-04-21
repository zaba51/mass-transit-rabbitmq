using Microsoft.EntityFrameworkCore;
using NotificationAPI.Entities;

namespace NotificationAPI.DbContexts
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.Channel).IsRequired();
                entity.Property(e => e.Recipient).IsRequired();
                entity.Property(e => e.TimeZone).IsRequired();
            });
        }
    }
}

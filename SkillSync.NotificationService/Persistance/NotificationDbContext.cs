using Microsoft.EntityFrameworkCore;
using SkillSync.NotificationService.DTOs;

namespace SkillSync.NotificationService.Persistance
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

        public DbSet<EmailLog> EmailLogs { get; set; }
    }
}

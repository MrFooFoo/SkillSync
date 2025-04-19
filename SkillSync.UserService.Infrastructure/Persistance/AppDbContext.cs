using Microsoft.EntityFrameworkCore;
using SkillSync.UserService.Domain.Entities;
namespace SkillSync.UserService.Infrastructure.Persistance;
public class AppDbContext : DbContext{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    public DbSet <User> Users => Set<User>();
    public DbSet <Skill> Skills => Set<Skill>();
}

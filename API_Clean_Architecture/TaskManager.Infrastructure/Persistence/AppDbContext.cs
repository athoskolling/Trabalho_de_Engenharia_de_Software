using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Global Query Filters for Soft Delete
        modelBuilder.Entity<User>().HasQueryFilter(u => u.DeletedAt == null);
        modelBuilder.Entity<TaskItem>().HasQueryFilter(t => t.DeletedAt == null);
        modelBuilder.Entity<Comment>().HasQueryFilter(c => c.TaskItem.DeletedAt == null && c.User.DeletedAt == null);
        modelBuilder.Entity<RefreshToken>().HasQueryFilter(r => r.User.DeletedAt == null);

        // Unique Index for Email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}
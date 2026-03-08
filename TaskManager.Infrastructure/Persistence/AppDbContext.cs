using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Security;

namespace TaskManager.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<TaskComment> TaskComments => Set<TaskComment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // USER
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Name)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(u => u.Email)
                  .IsRequired()
                  .HasMaxLength(150);

            entity.HasIndex(u => u.Email)
                  .IsUnique();

            entity.Property(u => u.Role)
                  .IsRequired()
                  .HasMaxLength(20);
        });

        // TASK
        modelBuilder.Entity<TaskItem>(entity =>
{
    entity.ToTable("Tasks"); // explicit table name

    entity.HasKey(t => t.Id);

    entity.Property(t => t.Id)
          .HasColumnType("uuid")
          .ValueGeneratedOnAdd(); // EF generates GUID

    entity.Property(t => t.Title)
          .IsRequired()
          .HasMaxLength(200);

    entity.Property(t => t.Priority)
          .HasConversion<int>();

    entity.Property(t => t.Status)
          .HasConversion<int>();

    entity.HasOne(t => t.User)
          .WithMany(u => u.Tasks)
          .HasForeignKey(t => t.UserId)
          .OnDelete(DeleteBehavior.Cascade);
});
        // TASK COMMENT
     modelBuilder.Entity<TaskComment>(entity =>
{
    entity.HasKey(c => c.Id);

    entity.Property(c => c.Comment)
          .IsRequired()
          .HasMaxLength(1000);

    entity.HasIndex(c => c.TaskItemId);

    entity.HasOne(c => c.User)
          .WithMany()
          .HasForeignKey(c => c.UserId)
          .OnDelete(DeleteBehavior.Cascade);
});
    
    }
}
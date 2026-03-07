using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public DateTime? DueDate { get; private set; }
    public Priority Priority { get; private set; }
    public TaskState Status { get; private set; }

    public Guid UserId { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    // Navigation property
    public User User { get; private set; }

    private TaskItem() { } // EF

    public TaskItem(string title, Guid userId, Priority priority)
    {
        Id = Guid.NewGuid();
        Title = title;
        UserId = userId;
        Priority = priority;
        Status = TaskState.Pending;
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public void Update(string title, string? description, DateTime? dueDate, Priority priority)
    {
        Title = title;
        Description = description;
        DueDate = dueDate?.ToUniversalTime();
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkCompleted()
    {
        Status = TaskState.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
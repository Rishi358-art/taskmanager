namespace TaskManager.Domain.Entities;

public class TaskComment
{
    public Guid Id { get; private set; }
    public Guid TaskItemId { get; private set; }
    public Guid UserId { get; private set; }
    public string Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation
    public User User { get; private set; }

    private TaskComment() { }

    public TaskComment(Guid taskItemId, Guid userId, string comment)
    {
        Id = Guid.NewGuid();
        TaskItemId = taskItemId;
        UserId = userId;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
    }
}
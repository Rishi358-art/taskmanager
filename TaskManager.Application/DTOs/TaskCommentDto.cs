namespace TaskManager.Application.DTOs;

public class TaskCommentDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
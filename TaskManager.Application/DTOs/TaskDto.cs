using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs;

public class TaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public TaskState Status { get; set; }
    public Priority Priority { get; set; }
}
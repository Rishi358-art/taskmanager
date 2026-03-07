using TaskManager.Domain.Entities;

namespace TaskManager.Application.Interfaces;

public interface ITaskCommentRepository
{
    Task AddAsync(TaskComment comment);
    Task<List<TaskComment>> GetByTaskIdAsync(Guid taskId);
    Task<Guid> DeleteAsync(Guid commentId);
}
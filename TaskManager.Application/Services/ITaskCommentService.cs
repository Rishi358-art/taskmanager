using TaskManager.Application.DTOs;

namespace TaskManager.Application.Services;

public interface ITaskCommentService
{
    Task AddCommentAsync(Guid taskId, Guid userId, string comment);
    Task<List<TaskCommentDto>> GetCommentsAsync(Guid taskId);
    Task DeleteCommentAsync(Guid commentId);
}
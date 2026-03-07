using TaskManager.Application.Interfaces;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services;

public class TaskCommentService : ITaskCommentService
{
    private readonly ITaskCommentRepository _repo;
    private readonly ICacheService _cacheService;
    public TaskCommentService(ITaskCommentRepository repo,ICacheService cacheService)
    {
        _repo = repo;
        _cacheService = cacheService;
    }

    public async Task AddCommentAsync(Guid taskId, Guid userId, string comment)
    {
        var entity = new TaskComment(taskId, userId, comment);
        await _repo.AddAsync(entity);
    }

    public async Task<List<TaskCommentDto>> GetCommentsAsync(Guid taskId)
{
    var comments = await _repo.GetByTaskIdAsync(taskId);

    return comments.Select(c => new TaskCommentDto
    {
        Id = c.Id,
        UserName = c.User?.Name ?? "Unknown",
        Comment = c.Comment,
        CreatedAt = c.CreatedAt
    }).ToList();
}
public async Task DeleteCommentAsync(Guid commentId)
{
    var taskId = await _repo.DeleteAsync(commentId);

    await _cacheService.RemoveAsync($"task_{taskId}");

    
}

}
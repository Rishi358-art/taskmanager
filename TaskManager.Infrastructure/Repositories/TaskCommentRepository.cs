using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.Repositories;

public class TaskCommentRepository : ITaskCommentRepository
{
    private readonly AppDbContext _context;

    public TaskCommentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TaskComment comment)
    {
        await _context.TaskComments.AddAsync(comment);
        await _context.SaveChangesAsync();
    }

   public async Task<List<TaskComment>> GetByTaskIdAsync(Guid taskId)
{
    return await _context.TaskComments
        .Include(c => c.User)
        .Where(c => c.TaskItemId == taskId)
        .ToListAsync();
}
public async Task<Guid> DeleteAsync(Guid commentId)
{
    var comment = await _context.TaskComments.FindAsync(commentId);

    if (comment == null)
        return Guid.Empty;

    var taskId = comment.TaskItemId;

    _context.TaskComments.Remove(comment);
    await _context.SaveChangesAsync();

    return taskId;
}
}
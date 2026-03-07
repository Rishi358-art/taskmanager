using TaskManager.Application.Interfaces;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using System.Text.Json;

namespace TaskManager.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IAuditService _auditService;
    private readonly ICacheService _cacheService;

    public TaskService(
        ITaskRepository taskRepository,
        IAuditService auditService,
        ICacheService cacheService)
    {
        _taskRepository = taskRepository;
        _auditService = auditService;
        _cacheService = cacheService;
    }

    // ==========================================
    // TASK LIST WITH SEARCH + PAGINATION CACHE
    // ==========================================
   public async Task<List<TaskDto>> GetUserTasksAsync(Guid userId)
{
    var cacheKey = $"tasks_{userId}";

    // var cached = await _cacheService.GetAsync<List<TaskDto>>(cacheKey);
    // if (cached != null)
    //     return cached;

    var tasks = await _taskRepository.GetUserTasksAsync(userId);

    var result = tasks
        .Where(t => !t.IsDeleted)
        .Select(t => new TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            Status = t.Status,
            Priority = t.Priority
        })
        .ToList();

    await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

    return result;
}
    public async Task<List<TaskDto>> GetUserTasksAsync(Guid userId, int page = 1, int pageSize = 10, string? search = null)
    {
        var cacheKey = $"tasks_{userId}_p{page}_s{search ?? ""}";

        try
        {
            var cached = await _cacheService.GetAsync<List<TaskDto>>(cacheKey);
           if (cached != null)
{
  
    return cached;
}
        }
        catch
        {
            // ignore cache errors (Redis down)
        }

        var tasks = await _taskRepository.GetUserTasksAsync(userId);

        var filtered = tasks.Where(t => !t.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            filtered = filtered.Where(t =>
                t.Title.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        var result = filtered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Status = t.Status,
                Priority = t.Priority
            })
            .ToList();

        try
        {
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
        }
        catch
        {
            // ignore cache errors
        }

        return result;
    }

    // ==========================================
    // TASK DETAILS WITH CACHE
    // ==========================================
    public async Task<TaskItem?> GetTaskByIdAsync(Guid taskId)
    {
        var cacheKey = $"task_{taskId}";

        try
        {
            var cached = await _cacheService.GetAsync<TaskItem>(cacheKey);
            if (cached != null)
                return cached;
        }
        catch
        {
            // ignore cache errors
        }

        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null) return null;

        try
        {
            await _cacheService.SetAsync(cacheKey, task, TimeSpan.FromMinutes(10));
        }
        catch { }

        return task;
    }

    // ==========================================
    // CREATE TASK (CACHE INVALIDATION)
    // ==========================================
    public async Task CreateTaskAsync(string title, string? description, DateTime? dueDate, Priority priority, Guid userId)
    {
        var task = new TaskItem(title, userId, priority);

        if (!string.IsNullOrWhiteSpace(description))
        {
            task.Update(title, description, dueDate, priority);
        }

        await _taskRepository.AddAsync(task);

        await _auditService.LogAsync("Created Task", "TaskItem", task.Id);

        try
        {
            await _cacheService.RemoveAsync($"tasks_{userId}");
        }
        catch { }
    }

    // ==========================================
    // UPDATE TASK (CACHE INVALIDATION)
    // ==========================================
    public async Task UpdateTaskAsync(Guid taskId, string title, string? description, DateTime? dueDate, Priority priority)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null) return;

        task.Update(title, description, dueDate, priority);

        await _taskRepository.UpdateAsync(task);

        await _auditService.LogAsync("Updated Task", "TaskItem", task.Id);

        try
        {
            await _cacheService.RemoveAsync($"tasks_{task.UserId}");
            await _cacheService.RemoveAsync($"task_{taskId}");
        }
        catch { }
    }

    // ==========================================
    // COMPLETE TASK
    // ==========================================
    public async Task CompleteTaskAsync(Guid taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null) return;

        task.MarkCompleted();

        await _taskRepository.UpdateAsync(task);

        await _auditService.LogAsync("Completed Task", "TaskItem", task.Id);

        try
        {
            await _cacheService.RemoveAsync($"tasks_{task.UserId}");
            await _cacheService.RemoveAsync($"task_{taskId}");
        }
        catch { }
    }

    // ==========================================
    // DELETE TASK (SOFT DELETE + CACHE CLEAR)
    // ==========================================
    public async Task DeleteTaskAsync(Guid taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null) return;

        await _taskRepository.SoftDeleteAsync(task);

        await _auditService.LogAsync("Deleted Task", "TaskItem", task.Id);

        try
        {
            await _cacheService.RemoveAsync($"tasks_{task.UserId}");
            await _cacheService.RemoveAsync($"task_{taskId}");
        }
        catch { }
    }
}
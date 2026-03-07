using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Services;

public interface ITaskService
{
   Task<List<TaskDto>> GetUserTasksAsync(Guid userId);
//    Task<List<TaskDto>> GetUserTasksAsync(Guid userId, int page, int pageSize);
   Task<List<TaskDto>> GetUserTasksAsync(Guid userId, int page, int pageSize, string? search);
    Task<TaskItem?> GetTaskByIdAsync(Guid taskId);
    Task CreateTaskAsync(string title, string? description, DateTime? dueDate, Priority priority, Guid userId);
    Task UpdateTaskAsync(Guid taskId, string title, string? description, DateTime? dueDate, Priority priority);
    Task CompleteTaskAsync(Guid taskId);
    Task DeleteTaskAsync(Guid taskId);
}
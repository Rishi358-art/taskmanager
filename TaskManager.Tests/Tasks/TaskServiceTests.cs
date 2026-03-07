using Moq;
using TaskManager.Application.Services;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

public class TaskServiceTests
{
    [Fact]
    public async Task CreateTask_Should_Call_Repository_AddAsync()
    {
        var repo = new Mock<ITaskRepository>();
        var audit = new Mock<IAuditService>();

        var service = new TaskService(repo.Object, audit.Object);

        await service.CreateTaskAsync(
            "Title",
            null,
            null,
            Priority.Medium,
            Guid.NewGuid()
        );

        repo.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTask_Should_Call_Repository_UpdateAsync()
    {
        var repo = new Mock<ITaskRepository>();
        var audit = new Mock<IAuditService>();

        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var task = new TaskItem(
            "Old Title",
            userId,
            Priority.Low
        );

        repo.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(task);

        var service = new TaskService(repo.Object, audit.Object);

        await service.UpdateTaskAsync(
            taskId,
            "New Title",
            null,
            null,
            Priority.High
        );

        repo.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTask_Should_Call_Repository_SoftDeleteAsync()
    {
        var repo = new Mock<ITaskRepository>();
        var audit = new Mock<IAuditService>();

        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var task = new TaskItem(
            "Title",
            userId,
            Priority.Medium
        );

        repo.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(task);

        var service = new TaskService(repo.Object, audit.Object);

        await service.DeleteTaskAsync(taskId);

        repo.Verify(r => r.SoftDeleteAsync(It.IsAny<TaskItem>()), Times.Once);
    }
}
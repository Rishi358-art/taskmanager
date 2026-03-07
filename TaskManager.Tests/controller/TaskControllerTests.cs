using Moq;
using TaskManager.Application.Services;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Web.ViewModels;
using Microsoft.AspNetCore.Http;

public class TasksControllerTests
{
    private TasksController CreateController(Mock<ITaskService> service)
    {
        var controller = new TasksController(service.Object, new Mock<ITaskCommentService>().Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        }, "mock"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        return controller;
    }

    [Fact]
    public async Task Create_Post_Should_Redirect_To_Index_On_Success()
    {
        var service = new Mock<ITaskService>();

        service.Setup(s => s.CreateTaskAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<DateTime?>(),
            It.IsAny<Priority>(),
            It.IsAny<Guid>()
        )).Returns(Task.CompletedTask);

        var controller = CreateController(service);

        var model = new TaskCreateViewModel
        {
            Title = "Test",
            Priority = Priority.Medium
        };

        var result = await controller.Create(model);

        Assert.IsType<RedirectToActionResult>(result);
    }

    [Fact]
    public async Task Index_Should_Return_View_With_Model()
    {
        var service = new Mock<ITaskService>();

        service.Setup(s => s.GetUserTasksAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<TaskDto>());

        var controller = CreateController(service);

        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsAssignableFrom<List<TaskViewModel>>(viewResult.Model);
    }
}
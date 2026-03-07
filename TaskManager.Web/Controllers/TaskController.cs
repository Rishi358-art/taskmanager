using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Services;
using TaskManager.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using TaskManager.Web.ViewModels;
using Microsoft.AspNetCore.RateLimiting;

[Authorize]
[EnableRateLimiting("fixed")]
public class TasksController : Controller
{
    private readonly ITaskService _taskService;
    private readonly ITaskCommentService _commentService;

    public TasksController(ITaskService taskService, ITaskCommentService commentService)
    {
        _taskService = taskService;
        _commentService = commentService;
    }

    private Guid CurrentUserId
    {
        get
        {
            var id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(id!);
        }
    }

   public async Task<IActionResult> Index(int page = 1, string? search = null)
{
    const int pageSize = 10;
    var cacheKey = $"tasks_{CurrentUserId}";

    // Try cached result first
    var allTasks = await _taskService.GetUserTasksAsync(CurrentUserId);

    // Apply search filter (no need to cache search results)
    if (!string.IsNullOrWhiteSpace(search))
    {
        allTasks = allTasks
            .Where(t => t.Title.Contains(search, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    var total = allTasks.Count;
    var pageCount = (int)Math.Ceiling(total / (double)pageSize);

    var paged = allTasks
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();

    var model = paged.Select(d => new TaskViewModel
    {
        Id = d.Id,
        Title = d.Title,
        Status = d.Status,
        Priority = d.Priority
    }).ToList();

    ViewBag.CurrentPage = page;
    ViewBag.PageCount = pageCount;
    ViewBag.Search = search;

    return View(model);
}
    public IActionResult Create() => View();

[HttpPost]
public async Task<IActionResult> Create(TaskCreateViewModel model)
{
    if (!ModelState.IsValid) return View(model);
  
    await _taskService.CreateTaskAsync(
        model.Title,
        model.Description,
        model.DueDate,
        model.Priority,
        CurrentUserId);
    TempData["Success"] = "Task created successfully";
    return RedirectToAction(nameof(Index));
}

    public async Task<IActionResult> Complete(Guid id)
    {
        await _taskService.CompleteTaskAsync(id);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        await _taskService.DeleteTaskAsync(id);
        TempData["Success"] = "Task deleted successfully";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        Console.WriteLine(task == null ? "TASK NULL" : "TASK FOUND");
        if (task == null) return NotFound();

        var comments = await _commentService.GetCommentsAsync(id);

        var model = new TaskDetailsViewModel
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Comments = comments.Select(c => new TaskCommentViewModel
            {
                Id = c.Id,
                UserName = c.UserName,
                Comment = c.Comment,
                CreatedAt = c.CreatedAt
            }).ToList()
        };

        return View(model);
    }
}
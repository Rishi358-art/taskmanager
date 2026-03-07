using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Services;

[Authorize]
public class TaskCommentsController : Controller
{
    private readonly ITaskCommentService _service;

    public TaskCommentsController(ITaskCommentService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Add(Guid taskId, string comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
        {
            return RedirectToAction("Details", "Tasks", new { id = taskId });
        }

        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

        await _service.AddCommentAsync(taskId, userId, comment);
        TempData["Success"] = "Comment added";
        return RedirectToAction("Details", "Tasks", new { id = taskId });
    }


[HttpPost]
public async Task<IActionResult> Delete(Guid commentId, Guid taskId)
{
    await _service.DeleteCommentAsync(commentId);

    TempData["Success"] = "Comment deleted";

    return RedirectToAction("Details", "Tasks", new { id = taskId });
}
}
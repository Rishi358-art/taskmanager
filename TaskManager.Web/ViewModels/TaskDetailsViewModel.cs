using System.ComponentModel.DataAnnotations;

namespace TaskManager.Web.ViewModels;

public class TaskDetailsViewModel
{
    public Guid Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string? Description { get; set; }

    public List<TaskCommentViewModel> Comments { get; set; } = new();
}
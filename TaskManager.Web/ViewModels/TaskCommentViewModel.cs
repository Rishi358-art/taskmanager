using System.ComponentModel.DataAnnotations;

namespace TaskManager.Web.ViewModels;

public class TaskCommentViewModel
{
    public Guid Id { get; set; }

    [Required]
    public string UserName { get; set; }

    [Required]
    [StringLength(1000)]
    public string Comment { get; set; }

    public DateTime CreatedAt { get; set; }
}
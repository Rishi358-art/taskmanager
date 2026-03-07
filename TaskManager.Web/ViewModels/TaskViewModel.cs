using System.ComponentModel.DataAnnotations;
using TaskManager.Domain.Enums;

namespace TaskManager.Web.ViewModels;

public class TaskViewModel
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    public TaskState Status { get; set; }

    public Priority Priority { get; set; }
}
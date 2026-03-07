using System.ComponentModel.DataAnnotations;
using TaskManager.Domain.Enums;

namespace TaskManager.Web.ViewModels;

public class TaskCreateViewModel
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200)]
    public string Title { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DueDate { get; set; }

    [Required]
    public Priority Priority { get; set; }
}
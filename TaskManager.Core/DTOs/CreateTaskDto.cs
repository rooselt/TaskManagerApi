using System.ComponentModel.DataAnnotations;
using TaskManager.Core.Enum;
using TaskStatus = TaskManager.Core.Enum.TaskStatus;

namespace TaskManager.Core.DTOs
{
    public class CreateTaskDto
    {
        [Required]
        [StringLength(100)]
        public string? Title { get; set; }

        [StringLength(500)]
        public required string Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public TaskPriority Priority { get; set; }

        [Required]
        public Guid UserId { get; set; }
    }


    public class UpdateTaskDto
    {
        [StringLength(100)]
        public required string Title { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskStatus? Status { get; set; }

        [Required]
        public Guid UserId { get; set; }
    }

}

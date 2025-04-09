using TaskManager.Core.Enum;
using TaskStatus = TaskManager.Core.Enum.TaskStatus;

namespace TaskManager.Core.DTOs
{
    public class TaskItemDto
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<CommentDto> Comments { get; set; } = [];     
        public IEnumerable<TaskHistoryDto> History { get; set; } = [];
    }
}

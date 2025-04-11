using TaskManager.Core.Enum;
using TaskManager.Core.Interfaces;
using TaskStatus = TaskManager.Core.Enum.TaskStatus;


namespace TaskManager.Core.Entities
{   

    public class TaskItem : ISoftDelete, IHasCreationTime
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskStatus Status { get; set; } // Enum: Pending, InProgress, Completed
        public TaskPriority Priority { get; set; } // Enum: Low, Medium, High
        public Guid ProjectId { get; set; }
        public Project? Project { get; set; }
        public ICollection<Comment> Comments { get; set; } = [];
        public ICollection<TaskHistory> History { get; set; } = [];
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public Guid? AssignedUserId { get; set; }
        public User? AssignedUser { get; set; }
        public Guid CreatedByUserId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }   

}

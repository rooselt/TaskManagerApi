using TaskManager.Core.Enum;

namespace TaskManager.Core.Entities
{

    public class TaskHistory
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public TaskItem? Task { get; set; }
        public Guid ChangedByUserId { get; set; }
        public User? ChangedByUser { get; set; }
        public required string ChangeDescription { get; set; }
        public DateTime ChangedAt { get; set; }

        public HistoryActionType ActionType { get; set; } // Enum com valores como Created, Updated, Deleted
    }
}

using TaskManager.Core.Enum;

namespace TaskManager.Core.DTOs
{
    public class TaskHistoryDto
    {
        public Guid Id { get; set; }
        public required string ChangeDescription { get; set; }
        public Guid ChangedByUserId { get; set; }
        public required string ChangedByUserName { get; set; }
        public DateTime ChangedAt { get; set; }
        public HistoryActionType ActionType { get; set; }
    }
}

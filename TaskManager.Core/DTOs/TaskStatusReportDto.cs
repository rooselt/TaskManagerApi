namespace TaskManager.Core.DTOs
{


    public class TaskStatusReportDto
    {
        public Guid ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public int TotalTasks { get; set; }
        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OverdueTasks { get; set; }
    }
}

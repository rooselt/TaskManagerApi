namespace TaskManager.Core.DTOs
{ 

   
    public class UserTasksReportDto
    {
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public int TotalAssignedTasks { get; set; }
        public int CompletedAssignedTasks { get; set; }
        public int PendingAssignedTasks { get; set; }
        public int OverdueAssignedTasks { get; set; }
        public int TotalCreatedTasks { get; set; }
        public List<UserTasksByProjectDto> TasksByProject { get; set; } = [];
    }

}

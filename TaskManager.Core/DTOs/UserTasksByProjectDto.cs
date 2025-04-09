namespace TaskManager.Core.DTOs
{

    public class UserTasksByProjectDto
    {
        public Guid ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public int TaskCount { get; set; }
    }
}

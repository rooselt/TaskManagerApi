

namespace TaskManager.Core.DTOs
{
    public class UserPerformanceDto
    {
        public Guid UserId { get; set; }
        public required string UserName { get; set; }
        public int TasksCompleted { get; set; }
        public int TasksCreated { get; set; }
    }
 
}


namespace TaskManager.Core.DTOs
{

    public class ProjectResponseDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TaskCount { get; set; }
    }

    public class ProjectDetailsDto : ProjectResponseDto
    {
        public required IEnumerable<TaskItemDto> Tasks { get; set; }
    }
}
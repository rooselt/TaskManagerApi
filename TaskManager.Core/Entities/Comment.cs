
namespace TaskManager.Core.Entities
{    
    public class Comment
    {
        public Guid Id { get; set; }
        public required string Content { get; set; }
        public Guid AuthorId { get; set; }
        public User? Author { get; set; }
        public Guid TaskId { get; set; }
        public TaskItem? Task { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

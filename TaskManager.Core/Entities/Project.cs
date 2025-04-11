using TaskManager.Core.Interfaces;

namespace TaskManager.Core.Entities
{
    public class Project : ISoftDelete, IHasCreationTime
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Guid OwnerId { get; set; }
        public User? Owner { get; set; }
        public ICollection<TaskItem> Tasks { get; set; } = [];
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }      
   
}

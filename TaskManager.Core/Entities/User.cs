using TaskManager.Core.Enum;
using TaskManager.Core.Interfaces;


namespace TaskManager.Core.Entities
{
    public class User : ISoftDelete, IHasCreationTime
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public UserRole Role { get; set; }
        public ICollection<Project> Projects { get; set; } = [];
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }

}

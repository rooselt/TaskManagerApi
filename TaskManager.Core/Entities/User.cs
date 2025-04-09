using TaskManager.Core.Enum;


namespace TaskManager.Core.Entities
{

    public class User
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public UserRole Role { get; set; }
        public ICollection<Project> Projects { get; set; } = [];
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}

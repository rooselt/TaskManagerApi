using TaskManager.Core.Entities;


namespace TaskManager.Core.Interfaces.Repository
{  
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetUserWithProjectsAsync(Guid userId);
        Task<bool> IsManagerAsync(Guid userId);
        Task<bool> ExistsAsync(Guid taskId);
    }
}

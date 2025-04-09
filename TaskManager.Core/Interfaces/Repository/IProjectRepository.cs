
using TaskManager.Core.Entities;


namespace TaskManager.Core.Interfaces.Repository
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<IEnumerable<Project>> GetUserProjectsAsync(Guid userId);
        Task<int> CountUserProjectsAsync(Guid userId);
        Task<bool> ExistsAsync(Guid taskId);
    }

}

using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces.Repository
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetCommentsByTaskAsync(Guid taskId);
        Task<int> CountCommentsByUserAsync(Guid userId);
        Task<Comment> AddCommentToTaskAsync(Guid taskId, Guid authorId, string content);
    }
}

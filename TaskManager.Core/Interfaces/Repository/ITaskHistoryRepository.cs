
using TaskManager.Core.Entities;
using TaskManager.Core.Enum;


namespace TaskManager.Core.Interfaces.Repository
{       
    public interface ITaskHistoryRepository : IRepository<TaskHistory>
    {
        Task<IEnumerable<TaskHistory>> GetByTaskIdAsync(Guid taskId);
        Task AddHistoryAsync(Guid taskId, Guid userId, string changeDescription, HistoryActionType actionType);
    }

}

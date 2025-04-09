using TaskManager.Core.Entities;
using TaskManager.Core.Enum;
using TaskStatus = TaskManager.Core.Enum.TaskStatus;


namespace TaskManager.Core.Interfaces.Repository
{
    
    public interface IHistoryService
    {
        Task RecordHistoryAsync(Guid taskId, Guid userId, string changeDescription, HistoryActionType actionType);
        Task<IEnumerable<TaskHistory>> GetTaskHistoryAsync(Guid taskId);
        Task RecordStatusChangeAsync(Guid taskId, Guid userId, TaskStatus oldStatus, TaskStatus newStatus);
        Task RecordCommentAddedAsync(Guid taskId, Guid userId, string commentContent);
    }
}

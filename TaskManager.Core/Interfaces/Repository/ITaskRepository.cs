using TaskManager.Core.Entities;
using TaskStatus = TaskManager.Core.Enum.TaskStatus;

namespace TaskManager.Core.Interfaces.Repository
{    
    public interface ITaskRepository : IRepository<TaskItem>
    {
        Task<TaskItem?> GetTaskAsync(Guid taskId);
        Task<int> CountTasksCreatedByUserAsync(Guid userId, DateTime startDate, DateTime endDate);
        Task<int> CountTasksByUserAndStatusAsync(Guid userId, TaskStatus status, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<TaskItem>> GetCompletedTasksByPeriodAsync(DateTime startDate, DateTime endDate, Guid? projectId = null);
        Task<IEnumerable<TaskItem>> GetTasksByProjectAsync(Guid projectId);
        Task<IEnumerable<TaskItem>> GetCompletedTasksLast30DaysAsync();   
        Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(Guid projectId, Enum.TaskStatus status);
        Task<IEnumerable<TaskItem>> GetTasksByUserAsync(Guid userId);
        Task<IEnumerable<TaskItem>> GetTasksCreatedByUserAsync(Guid userId);
        Task<bool> HasPendingTasksAsync(Guid projectId);
        Task<bool> ExistsAsync(Guid taskId);
        Task DeleteTaskAsync(Guid taskId);
    }
 
}

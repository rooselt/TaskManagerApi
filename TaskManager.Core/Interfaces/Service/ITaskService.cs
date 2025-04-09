namespace TaskManager.Core.Interfaces.Service
{
    using TaskManager.Core;
    using TaskManager.Core.DTOs;
    using TaskManager.Core.Entities;

    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetProjectTasks(Guid projectId);
        Task<TaskItem> CreateTask(Guid projectId, CreateTaskDto dto);
        Task<TaskItem> UpdateTask(Guid projectId, Guid taskId, UpdateTaskDto dto);
        Task DeleteTask(Guid projectId, Guid taskId);
        Task<Comment> AddCommentToTask(Guid taskId, AddCommentDto dto);
        Task<UserProductivityReportDto> GetUserProductivityReportAsync(Guid userId, DateTime startDate, DateTime endDate);
    }

}
using TaskManager.Core.DTOs;

namespace TaskManager.Core.Interfaces.Repository
{    
    public interface IReportService
    {
        Task<PerformanceReportDto> GetPerformanceReportAsync(Guid managerId, DateTime? startDate = null, DateTime? endDate = null);
        Task<ProjectProgressReportDto> GetProjectProgressReportAsync(Guid projectId);
        Task<UserTasksReportDto> GetUserTasksReportAsync(Guid userId);
        Task<IEnumerable<TaskStatusReportDto>> GetTasksStatusReportAsync();
    }
}

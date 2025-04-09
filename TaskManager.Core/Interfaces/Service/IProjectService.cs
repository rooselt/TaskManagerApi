namespace TaskManager.Core.Interfaces.Service
{
    using TaskManager.Core.DTOs;
    using TaskManager.Core.Models;
    using TaskManager.Core.Entities;
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetUserProjectsAsync(Guid userId);
        Task<Project> GetProjectByIdAsync(Guid projectId);
        Task<Project> CreateProjectAsync(CreateProjectDto createProjectDto);
        Task<OperationResult> DeleteProjectAsync(Guid projectId);
        Task<Project> UpdateProjectAsync(Guid projectId, UpdateProjectDto updateProjectDto);
        Task<int> CountUserProjectsAsync(Guid userId);
    }

}
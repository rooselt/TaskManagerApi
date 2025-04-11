using Microsoft.EntityFrameworkCore;
using TaskStatus = TaskManager.Core.Enum.TaskStatus;
using TaskManager.Infrastructure.Data;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces.Repository;
using TaskManager.Infrastructure.Repositories.Extensions;

namespace TaskManager.Infrastructure.Repositories
{
    
    public class ProjectRepository(AppDbContext context) : BaseRepository<Project>(context), IProjectRepository
    {
        public async Task<IEnumerable<Project>> GetUserProjectsAsync(Guid userId)
        {
            return await _context.Projects
                .Where(p => p.OwnerId == userId)
                .Include(p => p.Tasks)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> CountUserProjectsAsync(Guid userId)
        {
            return await _context.Projects
                .CountAsync(p => p.OwnerId == userId);
        }

        public async Task<bool> HasPendingTasksAsync(Guid projectId)
        {
            return await _context.Tasks
                .AnyAsync(t => t.ProjectId == projectId && t.Status == TaskStatus.Pending);
        }

        public async Task<int> CountTasksInProjectAsync(Guid projectId)
        {
            return await _context.Tasks
                .CountAsync(t => t.ProjectId == projectId);
        }

        public async Task<Project?> GetProjectWithTasksAsync(Guid projectId)
        {
            return await _context.Projects
                .Include(p => p.Tasks)
                .ThenInclude(t => t.Comments)
                .FirstOrDefaultAsync(p => p.Id == projectId);
        }

        public async Task DeleteProjectAsync(Guid projectId)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(t => t.Id == projectId)
                ?? throw new NotFoundException(nameof(TaskItem), projectId);

            _context.Projects.DeleteWithAudit(project);
            await _context.SaveChangesAsync();
        }
    }
}

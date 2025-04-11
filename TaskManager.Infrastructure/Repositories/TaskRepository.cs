using TaskStatus = TaskManager.Core.Enum.TaskStatus;
using TaskManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Interfaces.Repository;
using TaskManager.Core.Entities;
using System.Threading.Tasks;
using TaskManager.Infrastructure.Repositories.Extensions;

namespace TaskManager.Infrastructure.Repositories
{

    public class TaskRepository(AppDbContext context) : BaseRepository<TaskItem>(context), ITaskRepository
    {
        public async Task<IEnumerable<TaskItem>> GetTasksByProjectAsync(Guid projectId)
        {
            return await _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.Comments)
                .Include(t => t.History)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<TaskItem?> GetTaskAsync(Guid taskId)
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId);
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(Guid projectId, TaskStatus status)
        {
            return await _context.Tasks
                .Where(t => t.ProjectId == projectId && t.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetCompletedTasksLast30DaysAsync()
        {
            var dateThreshold = DateTime.UtcNow.AddDays(-30);
            return await _context.Tasks
                .Where(t => t.Status == TaskStatus.Completed && t.CompletedAt >= dateThreshold)
                .ToListAsync();
        }

        public async Task<bool> HasPendingTasksAsync(Guid projectId)
        {
            return await _context.Tasks
                .AnyAsync(t => t.ProjectId == projectId && t.Status == TaskStatus.Pending);
        }

        public async Task<int> CountTasksByProjectAsync(Guid projectId)
        {
            return await _context.Tasks
                .CountAsync(t => t.ProjectId == projectId);
        }

        public async Task<IEnumerable<TaskItem>> GetOverdueTasksAsync()
        {
            return await _context.Tasks
                .Where(t => t.DueDate < DateTime.UtcNow && t.Status != TaskStatus.Completed)
                .Include(t => t.Project)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByUserAsync(Guid userId)
        {
            return await _context.Tasks
                .Where(t => t.AssignedUserId == userId)
                .Include(t => t.Project)
                .Include(t => t.Comments)
                .OrderByDescending(t => t.DueDate)
                .ThenBy(t => t.Priority)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksCreatedByUserAsync(Guid userId)
        {
            return await _context.Tasks
                .Where(t => t.CreatedByUserId == userId)
                .Include(t => t.Project)
                .Include(t => t.AssignedUser)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetCompletedTasksByPeriodAsync(DateTime startDate, DateTime endDate, Guid? projectId = null)
        {
            var query = _context.Tasks
                .Where(t => t.Status == TaskStatus.Completed &&
                           t.CompletedAt >= startDate &&
                           t.CompletedAt <= endDate);

            if (projectId.HasValue)
            {
                query = query.Where(t => t.ProjectId == projectId.Value);
            }

            return await query
                .Include(t => t.Project)
                .Include(t => t.AssignedUser)
                .OrderByDescending(t => t.CompletedAt)
                .ThenBy(t => t.Priority)
                .ToListAsync();
        }

        public async Task<int> CountTasksCreatedByUserAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            return await _context.Tasks.Where(t => t.CreatedByUserId == userId &&
                    t.CreatedAt >= startDate &&
                    t.CreatedAt <= endDate).CountAsync();
        }

        public async Task<int> CountTasksByUserAndStatusAsync(Guid userId, TaskStatus status, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Tasks
                .Where(t => t.AssignedUserId == userId && t.Status == status);

            if (startDate.HasValue)
                query = query.Where(t => t.CompletedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.CompletedAt <= endDate.Value);

            return await query.CountAsync();
        }

        public async Task DeleteTaskAsync(Guid taskId)
        {
            var taskItem = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId)
                ?? throw new NotFoundException(nameof(TaskItem), taskId);

            _context.Tasks.DeleteWithAudit(taskItem);
        }
    }
}
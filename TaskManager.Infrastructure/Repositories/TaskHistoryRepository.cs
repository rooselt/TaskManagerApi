using TaskManager.Core.Enum;
using TaskManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces.Repository;

namespace TaskManager.Infrastructure.Repositories
{
    
    public class TaskHistoryRepository(AppDbContext context) : BaseRepository<TaskHistory>(context), ITaskHistoryRepository
    {
        public async Task<IEnumerable<TaskHistory>> GetByTaskIdAsync(Guid taskId)
        {
            return await _context.TaskHistories
                .Where(th => th.TaskId == taskId)
                .OrderByDescending(th => th.ChangedAt)
                .Include(th => th.ChangedByUser)
                .ToListAsync();
        }

        public async Task AddHistoryEntryAsync(Guid taskId, Guid userId, string changeDescription, HistoryActionType actionType)
        {
            var historyEntry = new TaskHistory
            {
                TaskId = taskId,
                ChangedByUserId = userId,
                ChangeDescription = changeDescription,
                ChangedAt = DateTime.UtcNow,
                ActionType = actionType
            };

            await _context.TaskHistories.AddAsync(historyEntry);
        }

        public async Task<IEnumerable<TaskHistory>> GetRecentActivityAsync(Guid projectId, int count = 10)
        {
            return await _context.TaskHistories
                .Where(th => th.Task != null && th.Task.ProjectId == projectId) 
                .OrderByDescending(th => th.ChangedAt)
                .Take(count)
                .Include(th => th.ChangedByUser)
                .Include(th => th.Task)
                .ToListAsync();
        }
              
        
        public async Task AddHistoryAsync(Guid taskId, Guid userId, string changeDescription, HistoryActionType actionType)
        {
            var history = new TaskHistory
            {
                TaskId = taskId,
                ChangedByUserId = userId,
                ChangeDescription = changeDescription,
                ChangedAt = DateTime.UtcNow,
                ActionType = actionType
            };

            await AddAsync(history);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces.Repository;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories
{
    public class CommentRepository(AppDbContext context) : BaseRepository<Comment>(context), ICommentRepository
    {
        public async Task<IEnumerable<Comment>> GetCommentsByTaskAsync(Guid taskId)
        {
            return await _context.Comments
                .Where(c => c.TaskId == taskId)
                .Include(c => c.Author)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> CountCommentsByUserAsync(Guid userId)
        {
            return await _context.Comments
                .CountAsync(c => c.AuthorId == userId);
        }

        public async Task<Comment> AddCommentToTaskAsync(Guid taskId, Guid authorId, string content)
        {
            var comment = new Comment
            {
                TaskId = taskId,
                AuthorId = authorId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Comments.AddAsync(comment);
            return comment;
        }
    }
}

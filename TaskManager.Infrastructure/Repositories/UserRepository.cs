using TaskManager.Core.Enum;
using TaskManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Interfaces.Repository;
using TaskManager.Core.Entities;

namespace TaskManager.Infrastructure.Repositories
{

    public class UserRepository(AppDbContext context) : BaseRepository<User>(context), IUserRepository
    {
        public async Task<User?> GetUserWithProjectsAsync(Guid userId)
        {
            return await _context.Users
                .Include(u => u.Projects)
                .ThenInclude(p => p.Tasks)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> IsManagerAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user?.Role == UserRole.Manager;
        }
     

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            return await _context.Users
                .Where(u => u.Role == role)
                .ToListAsync();
        }
    }
}

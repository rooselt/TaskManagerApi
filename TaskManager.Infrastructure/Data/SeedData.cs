using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskManager.Core.Entities;
using TaskManager.Core.Enum;

namespace TaskManager.Infrastructure.Data
{
    public static class SeedData
    {
        public static void ManagerUser(ModelBuilder modelBuilder, IConfiguration configuration)
        {
            var managerIdConfig = configuration["Manager:Id"];
            var managerNameConfig = configuration["Manager:Name"];
            var managerRoleConfig = configuration["Manager:Role"];
            var managerEmailConfig = configuration["Manager:Email"];

            if (!string.IsNullOrEmpty(managerIdConfig) &&
                !string.IsNullOrEmpty(managerNameConfig) &&
                !string.IsNullOrEmpty(managerRoleConfig) &&
                !string.IsNullOrEmpty(managerEmailConfig))
            {
                var managerId = Guid.Parse(managerIdConfig);
                var managerName = managerNameConfig;
                var managerRole = managerRoleConfig;
                var managerEmail = managerEmailConfig;

                modelBuilder.Entity<User>().HasData(new User
                {
                    Id = managerId,
                    Name = managerName,
                    Role = UserRole.Manager,
                    Email = managerEmail,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }
    }
}

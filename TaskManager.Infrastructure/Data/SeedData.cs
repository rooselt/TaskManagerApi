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

                modelBuilder.Entity<User>().HasData(new User
                {
                    Id = Guid.Parse(managerIdConfig),
                    Name = managerNameConfig,
                    Role = UserRole.Manager,
                    Email = managerEmailConfig,
                    CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                });
            }
        }
    }
}

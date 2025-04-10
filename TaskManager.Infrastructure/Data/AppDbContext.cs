
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TaskManager.Core.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaskManager.Core.Enum;

namespace TaskManager.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            this.Database.EnsureCreated();
        }
 
        public DbSet<User> Users => Set<User>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<TaskHistory> TaskHistories => Set<TaskHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = new Guid("3FA85F64-5717-4562-B3FC-2C963F66AFA6"),
                Name = "admin",
                Role = UserRole.Manager,                
                Email = "admin@taskmanager.com"
            });

            // Aplica todas as configurações do assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // Configurações globais de conversão
            configurationBuilder.Properties<DateTime>()
                .HaveConversion<DateTimeToUtcConverter>();

            configurationBuilder.Properties<string>()
                .HaveMaxLength(100);
        }
    }

    // Conversor para garantir datas em UTC
    public class DateTimeToUtcConverter : ValueConverter<DateTime, DateTime>
    {
        public DateTimeToUtcConverter()
            : base(
                v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
        {
        }
    }
}


using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;

namespace TaskManager.Infrastructure.Data.Configurations
{
    public class TaskHistoryConfiguration : IEntityTypeConfiguration<TaskHistory>
    {
        public void Configure(EntityTypeBuilder<TaskHistory> builder)
        {
            builder.ToTable("TaskHistories");

            builder.HasKey(th => th.Id);

            builder.Property(th => th.ChangeDescription)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(th => th.ChangedAt)
                .IsRequired()
                      .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(th => th.ActionType)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.HasOne(th => th.ChangedByUser)
                .WithMany()
                .HasForeignKey(th => th.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(th => th.Task)
                .WithMany(t => t.History)
                .HasForeignKey(th => th.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(th => new { th.TaskId, th.ChangedAt });

        }
    }
}

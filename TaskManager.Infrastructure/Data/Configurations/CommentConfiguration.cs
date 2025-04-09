using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;


namespace TaskManager.Infrastructure.Data.Configurations
{   
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comments");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(c => c.CreatedAt)
                .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.TaskId);

            builder.HasOne(c => c.Task)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

   
}

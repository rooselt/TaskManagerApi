using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Interfaces;

namespace TaskManager.Infrastructure.Data.Interceptor
{
    public class AuditingInterceptor : SaveChangesInterceptor
    {
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            // Ensure eventData.Context is not null before accessing ChangeTracker
            if (eventData.Context is null)
            {
                return await base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            var entries = eventData.Context.ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added && entry.Entity is IHasCreationTime created)
                {
                    created.CreatedAt = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Modified && entry.Entity is ISoftDelete deleted && deleted.IsDeleted)
                {
                    deleted.DeletedAt = DateTime.UtcNow;
                }
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}

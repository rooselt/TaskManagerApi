using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Interfaces;

namespace TaskManager.Infrastructure.Data.Interceptor
{
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            if (eventData.Context is null) return result;

            foreach (var entry in eventData.Context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added && e.Entity is ISoftDelete))
            {
                var entity = (ISoftDelete)entry.Entity;
                if (entity.IsDeleted)
                {
                    throw new InvalidOperationException(
                        $"Não é possível adicionar {entry.Entity.GetType().Name} marcado como deletado");
                }
            }

            return result;
        }
    }
}

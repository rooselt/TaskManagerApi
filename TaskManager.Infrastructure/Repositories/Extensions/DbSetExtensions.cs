using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Interfaces;

namespace TaskManager.Infrastructure.Repositories.Extensions
{
    public static class DbContextExtensions
    {
        public static void DeleteWithAudit<TEntity>(this DbSet<TEntity> dbSet, TEntity entity)
            where TEntity : class, ISoftDelete
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;

            dbSet.Update(entity); // Atualiza ao invés de deletar
        }

        public static void DeleteWithAudit<TEntity>(this DbSet<TEntity> dbSet, int id)
            where TEntity : class, ISoftDelete
        {
            var entity = dbSet.Find(id);
            if (entity != null)
            {
                dbSet.DeleteWithAudit(entity);
            }
        }
    }
}

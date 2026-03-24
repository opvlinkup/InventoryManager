using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Infrastructure.Database;

public static class DbContextConcurrencyExtensions
{
    public static void SetOriginalConcurrencyToken<TEntity, TProperty>(this DbContext context, TEntity entity, Expression<Func<TEntity, TProperty>> property, TProperty value)
        where TEntity : class
    {
        var entry = context.Entry(entity);

        entry.Property(property).OriginalValue = value!;
    }
}
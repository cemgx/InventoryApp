using InventoryApp.Application.Extensions;
using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Context;
using InventoryApp.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;

namespace InventoryApp.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly InventoryAppDbContext context;
        private readonly IMemoryCache _cache;
        private string CacheKey => $"{typeof(T).Name}_List";

        public Repository(InventoryAppDbContext context, IMemoryCache cache)
        {
            this.context = context;
            _cache = cache;
        }

        public async Task CreateAsync(T entity, CancellationToken cancellationToken)
        {
            await context.Set<T>().AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            ClearCache();
        }

        public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue(CacheKey, out List<T> cachedData))
            {
                cachedData = await ApplySoftDeleteFilter(context.Set<T>())
                              .AsNoTracking()
                              .ToListAsync(cancellationToken);
                _cache.Set(CacheKey, cachedData, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            return cachedData;
        }

        public async Task<T> GetByFilterAsync(Expression<Func<T, bool>> filter)
        {
            return await ApplySoftDeleteFilter(context.Set<T>())
                        .AsNoTracking()
                        .SingleOrDefaultAsync(filter);
        }

        public async Task<T> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await ApplySoftDeleteFilter(context.Set<T>())
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<T>> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            return await ApplySoftDeleteFilter(context.Set<T>())
                         .AsNoTracking()
                         .FilterByName(name)
                         .ToListAsync(cancellationToken);
        }

        public async Task<List<T>> GetAllIncludingDeletedAsync(CancellationToken cancellationToken)
        {
            return await context.Set<T>()
                         .AsNoTracking()
                         .ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            context.Set<T>().Update(entity);
            await context.SaveChangesAsync(cancellationToken);
            ClearCache();
        }

        public async Task SoftDeleteAsync(T entity, CancellationToken cancellationToken)
        {
            if (entity is BaseEntity softDelete)
            {
                softDelete.IsDeleted = true;
                await UpdateAsync(entity, cancellationToken);
            }
        }

        private void ClearCache()
        {
            _cache.Remove(CacheKey);
        }

        private IQueryable<T> ApplySoftDeleteFilter(IQueryable<T> query)
        {
            return typeof(BaseEntity).IsAssignableFrom(typeof(T))
                ? query.Where(x => x.IsDeleted == false)
                : query;
        }
    }
}
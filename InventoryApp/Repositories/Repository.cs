using InventoryApp.Application.Extensions;
using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Context;
using InventoryApp.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InventoryApp.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly InventoryAppDbContext context;

        public Repository(InventoryAppDbContext context)
        {
            this.context = context;
        }

        public async Task CreateAsync(T entity, CancellationToken cancellationToken)
        {
            await this.context.Set<T>().AddAsync(entity);
            await this.context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await ApplySoftDeleteFilter(context.Set<T>())
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);
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
            this.context.Set<T>().Update(entity);
            await this.context.SaveChangesAsync(cancellationToken);
        }

        public async Task SoftDeleteAsync(T entity, CancellationToken cancellationToken)
        {
            if (entity is BaseEntity softDelete)
            {
                softDelete.IsDeleted = true;
                await UpdateAsync(entity, cancellationToken);
            }
        }

        private IQueryable<T> ApplySoftDeleteFilter(IQueryable<T> query)
        {
            return typeof(BaseEntity).IsAssignableFrom(typeof(T))
                ? query.Where(x => x.IsDeleted == false)
                : query;
        }
    }
}
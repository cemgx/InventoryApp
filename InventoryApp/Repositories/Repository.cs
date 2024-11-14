using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Context;
using InventoryApp.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InventoryApp.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, new()
    {
        protected readonly InventoryAppDbContext context;

        public Repository(InventoryAppDbContext context)
        {
            this.context = context;
        }

        public async Task CreateAsync(T entity)
        {
            await this.context.Set<T>().AddAsync(entity);
            await this.context.SaveChangesAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await this.context.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByFilterAsync(Expression<Func<T, bool>> filter)
        {
            return await this.context.Set<T>().AsNoTracking().SingleOrDefaultAsync(filter);
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            return await this.context.Set<T>().FindAsync(id);
        }

        public async Task<List<T>> GetByNameAsync(string name)
        {
            return await this.context.Set<T>()
                .Where(e => EF.Property<string>(e, "Name").ToLower().Contains(name.ToLower()))
                .ToListAsync();
        }

        public async Task RemoveAsync(T entity)
        {
            this.context.Set<T>().Remove(entity);
            await this.context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            this.context.Set<T>().Update(entity);
            await this.context.SaveChangesAsync();
        }
    }
}
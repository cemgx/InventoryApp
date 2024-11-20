using InventoryApp.Models.Entity;
using System.Linq.Expressions;

namespace InventoryApp.Application.Interfaces
{
    public interface IRepository<T> where T : class, new()
    {
        Task<List<T>> GetAllAsync();

        Task<T> GetByIdAsync(object id);

        Task<T> GetByFilterAsync(Expression<Func<T, bool>> filter);

        Task CreateAsync(T entity);

        Task UpdateAsync(T entity);

        Task<List<T>> GetByNameAsync(string name);

        Task SoftDeleteAsync(T entity);

        Task<List<T>> GetAllIncludingDeletedAsync();
    }
}

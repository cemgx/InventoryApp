using InventoryApp.Models.Entity;
using System.Linq.Expressions;

namespace InventoryApp.Application.Interfaces
{
    public interface IRepository<T> where T : class, new()
    {
        Task<List<T>> GetAllAsync(CancellationToken cancellationToken);

        Task<T> GetByIdAsync(object id, CancellationToken cancellationToken);

        Task<T> GetByFilterAsync(Expression<Func<T, bool>> filter);

        Task CreateAsync(T entity, CancellationToken cancellationToken);

        Task UpdateAsync(T entity, CancellationToken cancellationToken);

        Task<List<T>> GetByNameAsync(string name, CancellationToken cancellationToken);

        Task SoftDeleteAsync(T entity, CancellationToken cancellationToken);

        Task<List<T>> GetAllIncludingDeletedAsync(CancellationToken cancellationToken);
    }
}

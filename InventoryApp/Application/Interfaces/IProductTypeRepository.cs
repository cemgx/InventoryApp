using InventoryApp.Models.Entity;
using InventoryApp.Repositories;

namespace InventoryApp.Application.Interfaces
{
    public interface IProductTypeRepository : IRepository<ProductType>
    {
        Task<List<ProductType>> GetByProductTypeIdAsync(int productTypeId);
    }
}

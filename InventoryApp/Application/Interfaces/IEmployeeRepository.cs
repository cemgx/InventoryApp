using InventoryApp.Models.Entity;

namespace InventoryApp.Application.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<List<Employee>> GetByEmployeeIdAsync(int employeeId);
    }
}

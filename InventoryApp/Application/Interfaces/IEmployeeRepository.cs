using InventoryApp.Application.LogEntities;
using InventoryApp.Models.Entity;

namespace InventoryApp.Application.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        string GenerateRandomString(int length);
        Task<List<Employee>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken);
        Task<Employee> GetByMailAsync(string mail, CancellationToken cancellationToken);
        Task LogEmployeeData(EmployeeLog employeeLog, CancellationToken cancellationToken);
    }
}

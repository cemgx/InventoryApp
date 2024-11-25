using InventoryApp.Models.Entity;

namespace InventoryApp.Application.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        string GenerateRandomString(int length);
        Task<List<Employee>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken);
        Task<Employee> GetByMailAsync(string mail, CancellationToken cancellationToken);
        Task<Employee> GetByForgotCodeAsync(string forgotCode, CancellationToken cancellationToken);
    }
}

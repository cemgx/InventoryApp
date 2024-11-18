using InventoryApp.Application.Extensions;
using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Context;
using InventoryApp.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace InventoryApp.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(InventoryAppDbContext context) : base(context)
        {
        }

        public async Task<List<Employee>> GetByEmployeeIdAsync(int employeeId)
        {
            return await this.context.Set<Employee>()
                .AsNoTracking()
                .FilterById(e => e.Id, employeeId)
                .ToListAsync();
        }
    }
}

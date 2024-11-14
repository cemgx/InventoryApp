using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Context;
using InventoryApp.Models.Entity;
using System.Reflection.Metadata.Ecma335;

namespace InventoryApp.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(InventoryAppDbContext context) : base(context)
        {
        }
    }
}

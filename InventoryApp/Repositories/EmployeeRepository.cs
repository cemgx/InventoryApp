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

        public async Task<List<Employee>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken)
        {
            return await this.context.Set<Employee>()
                .AsNoTracking()
                .FilterById(e => e.Id, employeeId)
                .ToListAsync(cancellationToken);
        }

        public async Task<Employee> GetByMailAsync(string mail, CancellationToken cancellationToken)
        {
            return await context.Employees
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Email == mail, cancellationToken);
        }

        private const string Charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public string GenerateRandomString(int length)
        {
            return string.Create<object?>(length, null,
                static (chars, _) => Random.Shared.GetItems(Charset, chars));
        }

    }
}

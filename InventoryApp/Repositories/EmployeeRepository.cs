using InventoryApp.Application.Extensions;
using InventoryApp.Application.Interfaces;
using InventoryApp.Application.LogEntities;
using InventoryApp.Models.Context;
using InventoryApp.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Compliance.Redaction;

namespace InventoryApp.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        private readonly ILogger<EmployeeRepository> _logger;
        private readonly Redactor _redactor;
        private readonly InventoryAppDbContext _context;
        public EmployeeRepository(ILogger<EmployeeRepository> logger, Redactor redactor, InventoryAppDbContext context)
            : base(context)
        {
            _logger = logger;
            _redactor = redactor;
            _context = context;
        }

        public async Task<List<Employee>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken)
        {
            return await _context.Set<Employee>()
                .AsNoTracking()
                .FilterById(e => e.Id, employeeId)
                .ToListAsync(cancellationToken);
        }

        public async Task<Employee> GetByMailAsync(string mail, CancellationToken cancellationToken)
        {
            return await _context.Employees
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Email == mail, cancellationToken);
        }

        private const string Charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public string GenerateRandomString(int length)
        {
            return string.Create<object?>(length, null,
                static (chars, _) => Random.Shared.GetItems(Charset, chars));
        }

        public async Task LogEmployeeData(EmployeeLog employeeLog, CancellationToken cancellationToken)
        {
            //employeeLog.Name = _redactor.Redact(employeeLog.Name);
            //employeeLog.Email = _redactor.Redact(employeeLog.Email);

            _logger.LogInformation("Employee Created: Name = {Name}, Email = {Email}",
                employeeLog.Name, employeeLog.Email);

            await Task.CompletedTask;
        }

    }
}

using InventoryApp.Application.Dto;
using InventoryApp.Models.Entity;

namespace InventoryApp.Application.MiddleWares
{
    public static partial class Logging
    {
        [LoggerMessage(LogLevel.Information, "Employee created")]
        public static partial void LogEmployeeCreated(
            this ILogger logger,
            [LogProperties] EmployeeRequestDto employee);
    }
}

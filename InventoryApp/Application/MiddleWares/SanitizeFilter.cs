using InventoryApp.Application.Dto;
using InventoryApp.Application.Utility;
using InventoryApp.Models.Entity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Compliance.Redaction;

namespace InventoryApp.Application.MiddleWares
{
    public class SanitizeFilter : IActionFilter
    {
        private readonly Redactor _redactor;

        public SanitizeFilter(Redactor redactor)
        {
            _redactor = redactor;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var argument in context.ActionArguments)
            {
                if (argument.Value is string stringValue)
                {
                    context.ActionArguments[argument.Key] = AntiXssUtility.Sanitize(stringValue);
                }
                else if (argument.Value is object dto)
                {
                    AntiXssUtility.SanitizeDto(dto);
                }
            }

            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument is EmployeeRequestDto employee)
                {
                    employee.Name = _redactor.Redact(employee.Name);
                    employee.Email = _redactor.Redact(employee.Email);
                }
            }
        }
        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
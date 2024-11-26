using InventoryApp.Application.Utility;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InventoryApp.Application.MiddleWares
{
    public class SanitizeFilter : IActionFilter
    {
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
        }
        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
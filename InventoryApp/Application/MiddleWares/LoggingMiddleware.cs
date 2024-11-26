using Microsoft.AspNetCore.Http;
using Serilog;
using System.Threading.Tasks;

namespace InventoryApp.Application.MiddleWares
{
    public class LoggingMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext context)
        {
            Log.Information("HTTP Request: {Method} {Path}", context.Request.Method, context.Request.Path);

            await _next(context);

            Log.Information("HTTP Response: {StatusCode}", context.Response.StatusCode);
        }
    }
}

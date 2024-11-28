using Serilog;
using System.Security.Cryptography;
using System.Text;

namespace InventoryApp.Application.MiddleWares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly ILogger<LoggingMiddleware> _logger;
        //private readonly string _secretKey;

        public LoggingMiddleware(RequestDelegate next/*, ILogger<LoggingMiddleware> logger, string secretKey*/)
        {
            _next = next;
            //_logger = logger;
            //_secretKey = secretKey;
        }

        public async Task Invoke(HttpContext context)
        {
            //if (context.Request.Headers.ContainsKey("mail"))
            //{
            //    var email = context.Request.Headers["mail"];
            //    var hashedEmail = HashData(email);

            //    _logger.LogInformation("Hashed Email: {HashedEmail}", hashedEmail);
            //}

            Log.Information("HTTP Request: {Method} {Path}", context.Request.Method, context.Request.Path);

            await _next(context);

            Log.Information("HTTP Response: {StatusCode}", context.Response.StatusCode);
        }

        //private string HashData(string data)
        //{
        //    using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
        //    var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        //    return Convert.ToBase64String(hashBytes);
        //}
    }
}

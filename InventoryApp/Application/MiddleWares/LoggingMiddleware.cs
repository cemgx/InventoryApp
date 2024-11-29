using Microsoft.Extensions.Compliance.Redaction;

namespace InventoryApp.Application.MiddleWares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;
        private readonly Redactor _redactor;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger, Redactor redactor)
        {
            _next = next;
            _logger = logger;
            _redactor = redactor;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method == HttpMethods.Post)
            {
                context.Request.EnableBuffering();
                var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
                context.Request.Body.Position = 0;

                var redactedBody = _redactor.Redact(body);

                _logger.LogInformation("Request Body (Redacted): {RedactedBody}", redactedBody);
            }

            await _next(context);
        }
    }
}
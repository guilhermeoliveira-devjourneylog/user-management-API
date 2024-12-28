using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace UserManagementAPI.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log the HTTP method and request path
            _logger.LogInformation($"Incoming Request: {context.Request.Method} {context.Request.Path}");

            var stopwatch = Stopwatch.StartNew();
            await _next(context); // Process the next middleware
            stopwatch.Stop();

            // Log the response status code
            _logger.LogInformation($"Outgoing Response: {context.Response.StatusCode}, Time Taken: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
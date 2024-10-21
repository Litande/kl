using System.Net;
using System.Text.Json;

namespace Plat4Me.DialAgentApi.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILoggerFactory factory)
    {
        _next = next;
        _logger = factory.CreateLogger<ExceptionHandlerMiddleware>();
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            _logger.LogError(error, "{Error}", error.Message);
            switch (error)
            {
                case KeyNotFoundException e:
                    // not found error
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                default:
                    // unhandled error
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
            }

            var result = JsonSerializer.Serialize(new { error = error?.Message });
            await response.WriteAsync(result);
        }
    }
}

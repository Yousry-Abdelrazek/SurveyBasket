using Microsoft.AspNetCore.Diagnostics;

namespace SurveyBasket.Errors;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Somethign went wrong : {Message}", exception.Message); 

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error!",
            Type = "https://httpstatuses.com/500"
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails , cancellationToken);
        return true;

    }
}

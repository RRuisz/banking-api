namespace BankingApi.Middleware;

using BankingApi.Exceptions;
using Microsoft.Extensions.Logging;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger )
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        } catch (NotFoundException e) 
        {
            _logger.LogError(e.Message);
            await HandleException(context, 404, e.Message);
        } catch (BadRequestException e)
        {
            _logger.LogError(e.Message);
            await HandleException(context, 400, e.Message);
        } catch (InvalidOperationsException e)
        {
            _logger.LogError(e.Message);
            await HandleException(context, 409, e.Message);
        } catch (Exception e)
        {
            _logger.LogError(e.Message);
            await HandleException(context, 500, "Internal Server error!");
        }
    }

    private static async Task HandleException(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new {
            status = statusCode,
            message = message
        });
    }
}
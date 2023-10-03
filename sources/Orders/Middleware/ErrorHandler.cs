using System.Net;
using System.Text.Json;

using Grpc.Core;

namespace Orders.Middlewar;

// NOTE: Very basic global exception handler, just to prevent having the full stack trace rendered to output
public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
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

            string? text;
            switch (error)
            {
                case RpcException e:
                    // custom application error
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    text = e.InnerException != null ? (e.InnerException?.Message) : (e?.Message);
                    break;
                default:
                    // unhandled error
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    text = error.InnerException != null ? (error.InnerException?.Message) : (error?.Message);
                    break;
            }

            var result = JsonSerializer.Serialize(new { message = text });
            await response.WriteAsync(result);
        }
    }
}
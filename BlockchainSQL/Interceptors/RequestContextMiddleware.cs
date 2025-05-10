using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace BlockchainSQL.Interceptors;

public class RequestCultureMiddleware
{
    private readonly RequestDelegate _next;

    public RequestCultureMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, RequestContext requestContext)
    {
        requestContext.IpAddress = context.Connection.RemoteIpAddress?.ToString();
        requestContext.Endpoint = context.Request.Path;

        await _next(context);
    }
}

public class RequestContext
{
    public string? IpAddress { get; set; }
    public string? Endpoint { get; set; }
}
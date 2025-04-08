using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

public class JwtBlacklistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDistributedCache _cache;

    public JwtBlacklistMiddleware(RequestDelegate next, IDistributedCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (token != null)
        {
            var isTokenBlacklisted = await _cache.GetStringAsync($"InvalidTokens:{token}");
            if (!string.IsNullOrEmpty(isTokenBlacklisted))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid token.");
                return;
            }
        }

        await _next(context);
    }
}

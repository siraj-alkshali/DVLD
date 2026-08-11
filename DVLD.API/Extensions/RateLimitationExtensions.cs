using System.Security.Claims;
using System.Threading.RateLimiting;

namespace DVLD.API.Extensions;

public static class RateLimitationExtensions
{
    public static IServiceCollection AddRateLimitationPolicies(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    string ip = GetClientIp(httpContext);

                    return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    });
                });

            options.AddPolicy("LoginLimiter", httpContext =>
            {
                string ip = GetClientIp(httpContext);

                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                });
            });

            options.AddPolicy("RefreshTokenLimiter", httpContext =>
            {
                string ip = GetClientIp(httpContext);

                return RateLimitPartition.GetFixedWindowLimiter(
                    ip,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    });
            });

            options.AddPolicy("PasswordLimiter", httpContext =>
            {
                string ip = GetClientIp(httpContext);

                string user =
                    httpContext.User.FindFirst(
                        ClaimTypes.NameIdentifier
                    )?.Value
                    ?? "anonymous";

                string partitionKey = $"{user}:{ip}";

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    });
            });

            options.AddPolicy("UsernameLimiter", httpContext =>
            {
                string ip = GetClientIp(httpContext);

                string user = GetUser(httpContext);

                string partitionKey = $"{user}:{ip}";

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    });
            });
        });

        return services;
    }

    private static string GetClientIp(HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";
    }

    private static string GetUser(HttpContext context)
    {
        return context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
    }
}
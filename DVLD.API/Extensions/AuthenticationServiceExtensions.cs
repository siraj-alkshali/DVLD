using DVLD.API.Services;
using DVLD.API.Services.Interfaces;

namespace DVLD.API.Extensions;

public static class AuthenticationServiceExtensions
{
    public static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
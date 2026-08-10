using System.Security.Claims;
using DVLD.API.Services.Interfaces;

namespace DVLD.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal User =>
        _httpContextAccessor.HttpContext?.User
        ?? throw new InvalidOperationException("HTTP context is unavailable.");

    public int UserID
    {
        get
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int id))
                throw new InvalidOperationException("Current user ID is unavailable");

            return id;
        }
    }

    public string UserName =>
        User.FindFirstValue(ClaimTypes.Name)
        ?? throw new InvalidOperationException("Current username is unavailable");
}
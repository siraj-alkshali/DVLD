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

    private ClaimsPrincipal? User =>
        _httpContextAccessor.HttpContext?.User;

    public int? UserID
    {
        get
        {
            string? userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.Parse(userId!);
        }
    }

    public string? UserName =>
        User?.FindFirst(ClaimTypes.Name)?.Value;
}
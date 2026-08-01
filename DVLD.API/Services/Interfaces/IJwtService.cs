using DVLD.DataAccess.Entities;

namespace DVLD.API.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
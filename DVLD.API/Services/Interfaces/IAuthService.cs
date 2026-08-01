using DVLD.API.Common.Results;
using DVLD.API.DTOs.Auth;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Services.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginRequestDto dto);
}
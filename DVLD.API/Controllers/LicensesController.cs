using DVLD.API.DTOs.Licenses;
using DVLD.API.Services.Interfaces;
using DVLD.API.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/licenses")]
public class LicensesController : ControllerBase
{
    private readonly ILicenseService _licenseService;

    public LicensesController(ILicenseService licenseService)
    {
        _licenseService = licenseService;
    }

    [HttpPost("first-time-license")]
    // [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LicenseDto>> CreateNewDrivingLicenseApplication(CreateLicenseDto dto)
    {
        ServiceResult<LicenseDto> result = await _licenseService.CreateNewLicenseAsync(dto);

        if (!result.IsSuccess)
            switch (result.ResultType)
            {
                case FailureType.Unauthorized:
                    return Unauthorized(result.Errors);
                case FailureType.Conflict:
                    return Conflict(result.Errors);
                default:
                    return StatusCode(500);
            }

        return Ok(result.Data);
    }
}
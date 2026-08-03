using DVLD.API.Common.QueryParameters;
using DVLD.API.Common.Results;
using DVLD.API.DTOs.Applications;
using DVLD.API.DTOs.Common;
using DVLD.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/applications")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationsController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<ApplicationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<ApplicationDto>>> GetAllApplications([FromQuery] ApplicationsQueryParameters parameters)
    {
        return Ok(await _applicationService.GetAllApplicationsAsync(parameters));
    }

    [HttpPost("new-local-driving-license-application")]
    // [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApplicationDto>> CreateNewDrivingLicenseApplication(CreateLocalDrivingLicenseApplicationDto dto)
    {
        ServiceResult<ApplicationDto> result = await _applicationService.CreateNewDrivingLicenseApplicationAsync(dto);

        if (!result.IsSuccess)
            return Conflict(result.Errors);

        return Ok(result.Data);
    }


}
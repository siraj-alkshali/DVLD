using DVLD.API.DTOs.ApplicationStatuses;
using DVLD.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/application-statuses")]
public class ApplicationStatusesController : ControllerBase
{
    private readonly IApplicationStatusService _applicationStatusService;

    public ApplicationStatusesController(IApplicationStatusService applicationStatusService)
    {
        _applicationStatusService = applicationStatusService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ApplicationStatusDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ApplicationStatusDto>>> GetAllApplicationStatuses()
    {
        return Ok(await _applicationStatusService.GetAllApplicationStatusesAsync());
    }

    [HttpGet("{applicationStatusId}", Name = "GetApplicationStatusById")]
    [ProducesResponseType(typeof(ApplicationStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationStatusDto>> GetApplicationStatusById(int applicationStatusId)
    {
        ApplicationStatusDto? applicationStatus = await _applicationStatusService.GetApplicationStatusByIdAsync(applicationStatusId);

        if (applicationStatus == null)
            return NotFound();

        return Ok(applicationStatus);
    }
}
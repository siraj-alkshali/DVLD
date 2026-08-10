using DVLD.API.Services.Interfaces;
using DVLD.API.DTOs.Tests;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/local-driving-license-applications")]
public class LocalDrivingLicenseApplicationsController : ControllerBase
{
    private readonly ITestService _testService;

    public LocalDrivingLicenseApplicationsController(ITestService testService)
    {
        _testService = testService;
    }

    [HttpGet("{localDrivingAppId}/tests")]
    [ProducesResponseType(typeof(List<TestListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TestListItemDto>>> GetAllTestForLocalDrivingApp(int localDrivingAppId)
    {
        return Ok(await _testService.GetAllTestsForLocalDrivingAppAsync(localDrivingAppId));
    }
}
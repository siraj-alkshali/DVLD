using DVLD.API.Services.Interfaces;
using DVLD.API.DTOs.Tests;
using DVLD.API.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/tests")]
public class TestsController : ControllerBase
{
    private readonly ITestService _testService;

    public TestsController(ITestService testService)
    {
        _testService = testService;
    }

    [HttpPost]
    // [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TestDto>> CreateNewTestAppointment(CreateTestDto dto)
    {
        ServiceResult<TestDto> result = await _testService.CreateNewTestResult(dto);

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
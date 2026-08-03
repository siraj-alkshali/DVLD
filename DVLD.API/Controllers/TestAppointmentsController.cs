using DVLD.API.Common.Results;
using DVLD.API.DTOs.Applications;
using DVLD.API.Services.Interfaces;
using DVLD.API.DTOs.TestAppointments;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/test-appointments")]
public class TestAppointmentsController : ControllerBase
{
    private readonly ITestAppointmentService _testAppointmentService;

    public TestAppointmentsController(ITestAppointmentService testAppointmentService)
    {
        _testAppointmentService = testAppointmentService;
    }

    [HttpPost]
    // [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TestAppointmentDto>> CreateNewTestAppointment(CreateTestAppointmentDto dto)
    {
        ServiceResult<TestAppointmentDto> result = await _testAppointmentService.CreateTestAppointmentAsync(dto);

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
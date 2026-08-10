using DVLD.API.Common.Results;
using DVLD.API.Extensions;
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

    [HttpGet("{testAppointmentId}", Name = "GetTestAppointmentById")]
    [ProducesResponseType(typeof(TestAppointmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TestAppointmentDto>> GetTestAppointmentById(int testAppointmentId)
    {
        TestAppointmentDto? testAppointment = await _testAppointmentService.GetTestAppointmentDtoByIdAsync(testAppointmentId);

        if (testAppointment == null)
            return NotFound();

        return Ok(testAppointment);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TestAppointmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TestAppointmentDto>> CreateTestAppointment(CreateTestAppointmentDto createTestAppointmentDto)
    {
        ServiceResult<TestAppointmentDto> result = await _testAppointmentService.CreateTestAppointmentAsync(createTestAppointmentDto);

        if (!result.IsSuccess)
            return this.ToActionResult(result);

        return CreatedAtRoute("GetTestAppointmentById", new { testAppointmentId = result.Data!.TestAppointmentID }, result.Data);
    }


}
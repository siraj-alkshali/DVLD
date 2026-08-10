using DVLD.API.Services.Interfaces;
using DVLD.API.DTOs.Tests;
using DVLD.API.Common.Results;
using DVLD.API.Extensions;
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

    [HttpGet("{testId}", Name = "GetTestById")]
    [ProducesResponseType(typeof(TestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TestDto>> GetTestById(int testId)
    {
        TestDto? test = await _testService.GetTestDtoByIdAsync(testId);

        if (test == null)
            return NotFound();

        return Ok(test);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TestDto>> CreateNewTestResult(CreateTestDto createTestDto)
    {
        ServiceResult<TestDto> result = await _testService.CreateNewTestResultAsync(createTestDto);

        if (!result.IsSuccess)
            return this.ToActionResult(result);

        return CreatedAtRoute("GetTestById", new { testId = result.Data!.TestID }, result.Data);
    }
}
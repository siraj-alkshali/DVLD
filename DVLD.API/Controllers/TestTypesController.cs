using DVLD.API.DTOs.TestTypes;
using DVLD.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/test-types")]
public class TestTypesController : ControllerBase
{

    private readonly ITestTypeService _testTypeService;

    public TestTypesController(ITestTypeService testTypeService)
    {
        _testTypeService = testTypeService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TestTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TestTypeDto>>> GetAllTestTypes()
    {
        return Ok(await _testTypeService.GetAllTestTypesAsync());
    }

    [HttpGet("{id}", Name = "GetTestTypeById")]
    [ProducesResponseType(typeof(TestTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TestTypeDto>> GetTestTypeById(int id)
    {
        TestTypeDto? testType = await _testTypeService.GetTestTypeDtoByIdAsync(id);

        if (testType == null)
            return NotFound();

        return Ok(testType);
    }
}
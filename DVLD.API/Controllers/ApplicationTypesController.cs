using DVLD.API.DTOs.ApplicationTypes;
using DVLD.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/application-types")]
public class ApplicationTypesController : ControllerBase
{
    private readonly IApplicationTypeService _applicationTypeService;

    public ApplicationTypesController(IApplicationTypeService applicationTypeService)
    {
        _applicationTypeService = applicationTypeService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ApplicationTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ApplicationTypeDto>>> GetAllApplicationTypes()
    {
        return Ok(await _applicationTypeService.GetAllApplicationTypesAsync());
    }
}
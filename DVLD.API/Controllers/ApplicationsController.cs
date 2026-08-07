using DVLD.API.Common.QueryParameters;
using DVLD.API.Common.Results;
using DVLD.API.DTOs.Applications;
using DVLD.API.DTOs.Common;
using DVLD.API.DTOs.TestAppointments;
using DVLD.API.Services.Interfaces;
using DVLD.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using DVLD.API.DTOs.Licenses;
using DVLD.API.DTOs;

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

    [HttpGet("{id}", Name = "GetApplicationById")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationDto>> GetApplicationById(int applicationId)
    {
        ApplicationDto? application = await _applicationService.GetApplicationDtoByIdAsync(applicationId);

        if (application == null)
            return NotFound();

        return Ok(application);
    }

    [HttpPost("new-local-driving-license-application")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationDto>> CreateNewDrivingLicenseApplication(CreateLocalDrivingLicenseApplicationDto createLocalDrivingLicenseApplicationDto)
    {
        ServiceResult<ApplicationDto> result = await _applicationService.CreateNewDrivingLicenseApplicationAsync(createLocalDrivingLicenseApplicationDto);

        if (!result.IsSuccess)
            return this.ToActionResult(result);

        return CreatedAtRoute("GetApplicationById", new { id = result.Data!.ApplicationID }, result.Data);
    }

    [HttpPost("retake-test-application")]
    [ProducesResponseType(typeof(TestAppointmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TestAppointmentDto>> CreateRetakeTestApplication(CreateRetakeTestApplicationDto createRetakeTestApplicationDto)
    {
        ServiceResult<TestAppointmentDto> result = await _applicationService.CreateNewRetakeTestApplication(createRetakeTestApplicationDto);

        if (!result.IsSuccess)
            return this.ToActionResult(result);

        return CreatedAtRoute("GetTestAppointmentById", new { id = result.Data!.TestAppointmentID }, result.Data);
    }

    [HttpPost("renew-license-application")]
    [ProducesResponseType(typeof(LicenseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LicenseDto>> RenewLicenseApplication(RenewLicenseDto renewLicenseDto)
    {
        ServiceResult<LicenseDto> result = await _applicationService.RenewDrivingLicenseAsync(renewLicenseDto);

        if (!result.IsSuccess)
            return this.ToActionResult(result);

        return CreatedAtRoute("GetLicenseById", new { id = result.Data!.LicenseID }, result.Data);
    }

    [HttpPost("replace-license-application")]
    [ProducesResponseType(typeof(LicenseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LicenseDto>> ReplaceLicenseApplication(ReplaceLicenseDto replaceLicenseDto)
    {
        ServiceResult<LicenseDto> result = await _applicationService.ReplaceDrivingLicenseAsync(replaceLicenseDto);

        if (!result.IsSuccess)
            return this.ToActionResult(result);

        return CreatedAtRoute("GetLicenseById", new { id = result.Data!.LicenseID }, result.Data);
    }

    [HttpPost("issue-international-license")]
    [ProducesResponseType(typeof(InternationalLicenseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LicenseDto>> IssueInternationalLicense(IssueInternationalLicenseDto issueInternationalLicenseDto)
    {
        ServiceResult<InternationalLicenseDto> result = await _applicationService.IssueInternationalLicenseAsync(issueInternationalLicenseDto);

        if (!result.IsSuccess)
            return this.ToActionResult(result);

        return CreatedAtRoute("GetInternationalLicenseById", new { id = result.Data!.InternationalLicenseID }, result.Data);
    }

}
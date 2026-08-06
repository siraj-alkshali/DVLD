using DVLD.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DVLD.API.Common.Files;
using DVLD.API.Common.Results;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/images")]
public class ImagesController : ControllerBase
{
    private readonly IImageService _imageService;

    public ImagesController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResult<string>>> UploadImage(IFormFile file)
    {
        ServiceResult<string> result = await _imageService.UploadImageAsync(file);

        if (!result.IsSuccess)
            return NotFound();

        return Ok(result.Data);
    }

    [HttpGet("{fileName}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetImage(string fileName)
    {
        FileResultData? file = _imageService.GetImage(fileName);

        if (file == null)
            return NotFound();

        return File(file.Stream, file.ContentType);
    }
}
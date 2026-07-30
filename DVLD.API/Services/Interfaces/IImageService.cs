using DVLD.API.Common.Files;
using DVLD.API.Common.Results;

namespace DVLD.API.Services.Interfaces;

public interface IImageService
{
    Task<ServiceResult<string>> UploadImageAsync(IFormFile file);
    FileResultData? GetImage(string fileName);
}
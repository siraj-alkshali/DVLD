using DVLD.API.Services.Interfaces;
using DVLD.API.Common.Files;
using DVLD.API.Common.Results;

namespace DVLD.API.Services;

public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ImageService(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
    {
        _environment = environment;
        _httpContextAccessor = httpContextAccessor;
    }

    private async Task<bool> IsValidImageContentAsync(IFormFile file)
    {
        using Stream stream = file.OpenReadStream();

        byte[] buffer = new byte[8];

        await stream.ReadAsync(buffer, 0, buffer.Length);

        return buffer.Take(3).SequenceEqual(ImageSettings.JpegSignature)
        || buffer.SequenceEqual(ImageSettings.PngSignature);
    }

    private async Task<ServiceResult> ValidateImage(IFormFile file)
    {
        string fileExtension = Path.GetExtension(file.FileName).ToLower();

        if (!ImageSettings.AllowedExtensions.Contains(fileExtension))
            return ServiceResult.Failure(["Only jpg, jpeg and png images are allowed"], FailureType.BadRequest);

        if (file.Length > ImageSettings.MaxSizeInBytes)
            return ServiceResult.Failure(["File size should not exceed 4 MB"], FailureType.BadRequest);

        if (!await IsValidImageContentAsync(file))
            return ServiceResult.Failure(["The uploaded file is not a valid image"], FailureType.BadRequest);

        return ServiceResult.Success();
    }

    public async Task<ServiceResult<string>> UploadImageAsync(IFormFile file)
    {
        ServiceResult uploadImageValidation = await ValidateImage(file);

        if (!uploadImageValidation.IsSuccess)
            return ServiceResult<string>.Failure(uploadImageValidation.Errors, FailureType.ValidationError);

        string imagesFolder = Path.Combine(_environment.WebRootPath, "images");

        if (!Directory.Exists(imagesFolder))
            Directory.CreateDirectory(imagesFolder);

        string fileExtension = Path.GetExtension(file.FileName);

        string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

        string filePath = Path.Combine(imagesFolder, uniqueFileName);

        using FileStream stream = new FileStream(filePath, FileMode.Create);

        await file.CopyToAsync(stream);

        return ServiceResult<string>.Success(uniqueFileName);
    }

    public FileResultData? GetImage(string fileName)
    {
        string imagePath = Path.Combine(_environment.WebRootPath, "images", fileName);

        if (!File.Exists(imagePath))
            return null;

        FileStream stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);

        string contentType = GetContentType(imagePath);

        return new FileResultData
        {
            Stream = stream,
            ContentType = contentType
        };
    }

    private string GetContentType(string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLower();

        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };
    }

    public Task DeleteImage(string fileName)
    {
        string imagePath = Path.Combine(_environment.WebRootPath, "images", fileName);

        if (File.Exists(imagePath))
            File.Delete(imagePath);

        return Task.CompletedTask;
    }

    public string? GetImageUrl(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            return null;

        HttpRequest request = _httpContextAccessor.HttpContext!.Request;

        return $"{request.Scheme}://{request.Host}/images/{imagePath}";
    }
}
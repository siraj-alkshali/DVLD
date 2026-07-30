namespace DVLD.API.Common.Files;

public static class ImageSettings
{
    public static readonly string[] AllowedExtensions =
    {
        ".jpg",
        ".jpeg",
        ".png"
    };

    public const long MaxSizeInBytes = 4 * 1024 * 1024;

    public static readonly byte[] JpegSignature =
    {
    0xFF, 0xD8, 0xFF
    };

    public static readonly byte[] PngSignature =
    {
    0x89, 0x50, 0x4E, 0x47,
    0x0D, 0x0A, 0x1A, 0x0A
    };
}
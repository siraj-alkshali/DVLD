namespace DVLD.API.DTOs;

public class ReplaceLicenseDto
{
    public int LicenseID { get; set; }
    public string? Notes { get; set; }
    public enReplacementReason ReplacementReason { get; set; }

    public enum enReplacementReason
    {
        ReplaceLostLicense = 1,
        ReplaceDamagedLicense = 2
    }

}
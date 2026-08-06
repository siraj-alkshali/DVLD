using DVLD.API.Common.Constants;

namespace DVLD.API.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName(this enRoleType roleType)
    {
        return roleType switch
        {
            enRoleType.Admin => "Admin",
            enRoleType.Employee => "Employee",
            _ => roleType.ToString()
        };
    }

    public static string GetDisplayName(this enTestType testType)
    {
        return testType switch
        {
            enTestType.VisionTest => "Vision Test",
            enTestType.WrittenTheoryTest => "Written Test",
            enTestType.PracticalStreetTest => "Street Test",
            _ => testType.ToString()
        };
    }

    public static string GetDisplayName(this enApplicationType applicationType)
    {
        return applicationType switch
        {
            enApplicationType.NewLocalDrivingLicense => "New Local Driving License",
            enApplicationType.RenewDrivingLicense => "Renew Driving License",
            enApplicationType.ReplacementForLostDrivingLicense => "Replacement for Lost Driving License",
            enApplicationType.ReplacementForDamagedDrivingLicense => "Replacement for Damaged Driving License",
            enApplicationType.ReleaseDetainedDrivingLicense => "Release Detained Driving License",
            enApplicationType.NewInternationalLicense => "New International License",
            enApplicationType.RetakeTest => "Retake Test",
            _ => applicationType.ToString()
        };
    }

    public static string GetDisplayName(this enLicenseIssueReason issueReason)
    {
        return issueReason switch
        {
            enLicenseIssueReason.FirstTimeIssue => "First Time License Issue",
            enLicenseIssueReason.Renewal => "Renewal",
            enLicenseIssueReason.ReplacementForLostLicense => "Replacement for Lost Driving License",
            enLicenseIssueReason.ReplacementForDamagedLicense => "Replacement for Damaged Driving License",
            _ => issueReason.ToString()
        };
    }

}
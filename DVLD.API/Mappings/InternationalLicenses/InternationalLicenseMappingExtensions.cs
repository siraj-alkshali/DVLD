using DVLD.API.DTOs;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Mappings.People;

public static class InternationalLicenseMappingExtensions
{
    public static InternationalLicenseDto ToDto(this InternationalLicense internationalLicense)
    {
        return new InternationalLicenseDto(
            internationalLicense.InternationalLicenseID,
            $"{internationalLicense.Driver.Person.FirstName} {internationalLicense.Driver.Person.LastName}",
            internationalLicense.Driver.Person.NationalNo,
            internationalLicense.Driver.Person.Phone,
            internationalLicense.IssuedUsingLocalLicense.LicenseClass.ClassName,
            internationalLicense.IssueDate,
            internationalLicense.ExpirationDate,
            internationalLicense.IsActive
        );
    }
}
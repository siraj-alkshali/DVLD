using DVLD.API.DTOs.Licenses;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Mappings.Applications;

public static class LicenseMappingExtensions
{
    public static LicenseDto ToDto(this License license)
    {
        return new LicenseDto(
            license.LicenseID,
            $"{license.Driver.Person.FirstName} {license.Driver.Person.LastName}",
            license.Driver.Person.NationalNo,
            license.Driver.Person.Phone,
            license.LicenseClass.ClassName,
            license.LicenseIssueReason.IssueReasonName,
            license.IssueDate,
            license.ExpirationDate,
            license.IsActive
        );
    }
}
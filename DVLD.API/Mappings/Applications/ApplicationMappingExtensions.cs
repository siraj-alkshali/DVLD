using DVLD.API.DTOs.Applications;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Mappings.Applications;

public static class ApplicationMappingExtensions
{
    public static ApplicationDto ToDto(this Application application)
    {
        return new ApplicationDto(
            application.ApplicationID,
            $"{application.ApplicantPerson.FirstName} {application.ApplicantPerson.LastName}",
            application.ApplicantPerson.NationalNo,
            application.ApplicationDate,
            application.ApplicationType.ApplicationTypeTitle,
            application.ApplicationStatus.StatusName,
            application.CreatedByUser.UserName
        );
    }
}
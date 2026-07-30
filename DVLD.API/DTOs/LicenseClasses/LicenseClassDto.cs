namespace DVLD.API.DTOs.LicenseClasses;

public class LicenseClassDto
{

    public LicenseClassDto(int licenseClassId, string className, string classDescription, byte minimumAllowedAge, byte defaultValidityLength, decimal classFees)
    {
        LicenseClassID = licenseClassId;
        ClassName = className;
        ClassDescription = classDescription;
        MinimumAllowedAge = minimumAllowedAge;
        DefaultValidityLength = defaultValidityLength;
        ClassFees = classFees;
    }

    public int LicenseClassID { get; set; }
    public string ClassName { get; set; }
    public string ClassDescription { get; set; }
    public byte MinimumAllowedAge { get; set; }
    public byte DefaultValidityLength { get; set; }
    public decimal ClassFees { get; set; }
}
namespace DVLD.API.Common.QueryParameters;

public class InternationalLicensesQueryParameters : SearchParameters
{
    public int? LicenseClassID { get; set; }
    public bool? IsActive { get; set; }
    public DateOnly? IssueDateFrom { get; set; }
    public DateOnly? IssueDateTo { get; set; }
    public DateOnly? ExpirationDateFrom { get; set; }
    public DateOnly? ExpirationDateTo { get; set; }
}
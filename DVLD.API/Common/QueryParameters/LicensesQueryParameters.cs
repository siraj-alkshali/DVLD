namespace DVLD.API.Common.QueryParameters;

public class LicenseQueryParameters : SearchParameters
{
    public int? LicenseClassID { get; set; }
    public int? IssueReasonID { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
    public bool? IsActive { get; set; }
}
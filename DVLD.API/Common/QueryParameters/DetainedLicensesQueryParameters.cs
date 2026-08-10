using Microsoft.Net.Http.Headers;

namespace DVLD.API.Common.QueryParameters;

public class DetainedLicensesQueryParameters : SearchParameters
{
    public bool? IsReleased { get; set; }
    public DateOnly? DetainDateFrom { get; set; }
    public DateOnly? DetainDateTo { get; set; }
}
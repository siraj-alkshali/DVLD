using DVLD.API.Common.QueryParameters;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Extensions;

public static class InternationalLicenseQueryExtensions
{
    public static IQueryable<InternationalLicense> ApplySearch(this IQueryable<InternationalLicense> query, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        searchTerm = searchTerm.Trim().ToLower();

        return query.Where(intlLicense => intlLicense.Driver.Person.FirstName.Contains(searchTerm)
        || intlLicense.Driver.Person.LastName.Contains(searchTerm)
        || intlLicense.Driver.Person.NationalNo == searchTerm
        || intlLicense.Driver.Person.Phone == searchTerm);
    }

    public static IQueryable<InternationalLicense> ApplyFilter(this IQueryable<InternationalLicense> query, InternationalLicensesQueryParameters? parameters)
    {
        if (parameters == null)
            return query;

        if (parameters.IsActive.HasValue)
            query = query.Where(intlLicense => intlLicense.IsActive == parameters.IsActive)
;
        if (parameters.LicenseClassID.HasValue)
            query = query.Where(intlLicense => intlLicense.IssuedUsingLocalLicense.LicenseClass.LicenseClassID == parameters.LicenseClassID);

        if (parameters.IssueDateFrom.HasValue)
            query = query.Where(intlLicense => intlLicense.IssueDate >= parameters.IssueDateFrom);

        if (parameters.IssueDateTo.HasValue)
            query = query.Where(intlLicense => intlLicense.IssueDate <= parameters.IssueDateTo);

        if (parameters.ExpirationDateFrom.HasValue)
            query = query.Where(intlLicense => intlLicense.ExpirationDate >= parameters.ExpirationDateFrom);

        if (parameters.ExpirationDateTo.HasValue)
            query = query.Where(intlLicense => intlLicense.ExpirationDate <= parameters.ExpirationDateTo);

        return query;
    }

    public static IQueryable<InternationalLicense> ApplySort(this IQueryable<InternationalLicense> query, InternationalLicensesQueryParameters parameters)
    {
        bool descending = string.Equals(parameters.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return parameters.SortBy?.ToLower() switch
        {
            "firstname" =>
                descending
                    ? query.OrderByDescending(intlLicense => intlLicense.Driver.Person.FirstName)
                        .ThenByDescending(intlLicense => intlLicense.InternationalLicenseID)

                    : query.OrderBy(intlLicense => intlLicense.Driver.Person.FirstName)
                        .ThenBy(intlLicense => intlLicense.InternationalLicenseID),

            "lastname" =>
                descending
                    ? query.OrderByDescending(intlLicense => intlLicense.Driver.Person.LastName)
                        .ThenByDescending(intlLicense => intlLicense.InternationalLicenseID)

                    : query.OrderBy(intlLicense => intlLicense.Driver.Person.LastName)
                        .ThenBy(intlLicense => intlLicense.InternationalLicenseID),

            "issuedate" =>
                descending
                    ? query.OrderByDescending(intlLicense => intlLicense.IssueDate)
                        .ThenByDescending(intlLicense => intlLicense.InternationalLicenseID)

                    : query.OrderBy(intlLicense => intlLicense.IssueDate)
                        .ThenBy(intlLicense => intlLicense.InternationalLicenseID),

            "expirationdate" =>
                descending
                    ? query.OrderByDescending(intlLicense => intlLicense.ExpirationDate)
                        .ThenByDescending(intlLicense => intlLicense.InternationalLicenseID)

                    : query.OrderBy(intlLicense => intlLicense.ExpirationDate)
                        .ThenBy(intlLicense => intlLicense.InternationalLicenseID),

            _ =>
                query.OrderBy(intlLicense => intlLicense.InternationalLicenseID)
        };
    }
}
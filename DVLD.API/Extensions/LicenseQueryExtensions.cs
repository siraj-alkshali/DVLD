using DVLD.API.Common.QueryParameters;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Extensions;

public static class LicenseQueryExtensions
{
    public static IQueryable<License> ApplySearch(this IQueryable<License> query, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        searchTerm = searchTerm.Trim();

        return query.Where(l => l.Application.ApplicantPerson.NationalNo == searchTerm
        || l.Application.ApplicantPerson.FirstName.Contains(searchTerm)
        || l.Application.ApplicantPerson.LastName.Contains(searchTerm));
    }

    public static IQueryable<License> ApplyFilter(this IQueryable<License> query, LicenseQueryParameters? parameters)
    {
        if (parameters == null)
            return query;

        if (parameters.IsActive.HasValue)
            query = query.Where(l => l.IsActive == parameters.IsActive);

        if (parameters.IssueReasonID.HasValue)
            query = query.Where(l => l.IssueReasonID == parameters.IssueReasonID);

        if (parameters.LicenseClassID.HasValue)
            query = query.Where(l => l.LicenseClassID == parameters.LicenseClassID);

        if (parameters.DateFrom.HasValue)
            query = query.Where(l => l.IssueDate >= parameters.DateFrom);

        if (parameters.DateTo.HasValue)
            query = query.Where(l => l.ExpirationDate <= parameters.DateTo);

        return query;
    }

    public static IQueryable<License> ApplySort(this IQueryable<License> query, LicenseQueryParameters parameters)
    {
        bool descending = string.Equals(parameters.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return parameters.SortBy?.ToLower() switch
        {
            "firstname" =>
                descending
                    ? query.OrderByDescending(l => l.Application.ApplicantPerson.FirstName)
                        .ThenByDescending(l => l.LicenseID)

                    : query.OrderBy(l => l.Application.ApplicantPerson.FirstName)
                        .ThenByDescending(l => l.LicenseID),

            "lastname" =>
                descending
                    ? query.OrderByDescending(l => l.Application.ApplicantPerson.LastName)
                        .ThenByDescending(l => l.LicenseID)

                    : query.OrderBy(l => l.Application.ApplicantPerson.LastName)
                        .ThenByDescending(l => l.LicenseID),

            "issuedate" =>
                descending
                    ? query.OrderByDescending(l => l.IssueDate)
                        .ThenByDescending(l => l.LicenseID)

                    : query.OrderBy(l => l.IssueDate)
                        .ThenByDescending(l => l.LicenseID),

            "expirationdate" =>
                descending
                    ? query.OrderByDescending(l => l.ExpirationDate)
                        .ThenByDescending(l => l.LicenseID)

                    : query.OrderBy(l => l.ExpirationDate)
                        .ThenByDescending(l => l.LicenseID),

            _ =>
                query.OrderBy(l => l.LicenseID)
        };
    }
}
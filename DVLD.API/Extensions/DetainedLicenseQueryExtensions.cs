using DVLD.API.Common.QueryParameters;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Extensions;

public static class DetainedLicenseQueryExtensions
{
    public static IQueryable<DetainedLicense> ApplySearch(this IQueryable<DetainedLicense> query, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        searchTerm = searchTerm.Trim().ToLower();

        return query.Where(dl => dl.License.Driver.Person.FirstName.Contains(searchTerm)
        || dl.License.Driver.Person.LastName.Contains(searchTerm)
        || dl.License.Driver.Person.NationalNo == searchTerm
        || dl.License.Driver.Person.Phone == searchTerm);
    }

    public static IQueryable<DetainedLicense> ApplyFilter(this IQueryable<DetainedLicense> query, DetainedLicensesQueryParameters? parameters)
    {
        if (parameters == null)
            return query;

        if (parameters.IsReleased.HasValue)
            query = parameters.IsReleased.Value
            ? query.Where(dl => dl.ReleaseDate != null)
            : query.Where(dl => dl.ReleaseDate == null);

        if (parameters.DetainDateFrom.HasValue)
            query = query.Where(dl => dl.DetainDate >= parameters.DetainDateFrom);

        if (parameters.DetainDateTo.HasValue)
            query = query.Where(dl => dl.DetainDate <= parameters.DetainDateTo);

        return query;
    }

    public static IQueryable<DetainedLicense> ApplySort(this IQueryable<DetainedLicense> query, DetainedLicensesQueryParameters parameters)
    {
        bool descending = string.Equals(parameters.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return parameters.SortBy?.ToLower() switch
        {
            "firstname" =>
                descending
                    ? query.OrderByDescending(dl => dl.License.Driver.Person.FirstName)
                        .ThenByDescending(dl => dl.DetainID)

                    : query.OrderBy(dl => dl.License.Driver.Person.FirstName)
                        .ThenBy(dl => dl.DetainID),


            "lastname" =>
                descending
                    ? query.OrderByDescending(dl => dl.License.Driver.Person.LastName)
                        .ThenByDescending(dl => dl.DetainID)

                    : query.OrderBy(dl => dl.License.Driver.Person.LastName)
                        .ThenBy(dl => dl.DetainID),

            "finefees" =>
                descending
                    ? query.OrderByDescending(dl => dl.FineFees)
                        .ThenByDescending(dl => dl.DetainID)

                    : query.OrderBy(dl => dl.FineFees)
                        .ThenBy(dl => dl.DetainID),

            "detaindate" =>
                descending
                    ? query.OrderByDescending(dl => dl.DetainDate)
                        .ThenByDescending(dl => dl.DetainID)

                    : query.OrderBy(dl => dl.DetainDate)
                        .ThenBy(dl => dl.DetainID),

            "releasedate" =>
                descending
                    ? query.OrderByDescending(dl => dl.ReleaseDate)
                        .ThenByDescending(dl => dl.DetainID)

                    : query.OrderBy(dl => dl.ReleaseDate)
                        .ThenBy(dl => dl.DetainID),

            _ =>
                query.OrderBy(dl => dl.DetainID)
        };
    }
}
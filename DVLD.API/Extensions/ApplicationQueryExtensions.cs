using DVLD.API.Common.QueryParameters;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Extensions;

public static class ApplicationQueryExtensions
{
    public static IQueryable<Application> ApplySearch(this IQueryable<Application> query, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        searchTerm = searchTerm.Trim().ToLower();

        return query.Where(app => app.ApplicantPerson.FirstName.Contains(searchTerm)
        || app.ApplicantPerson.LastName.Contains(searchTerm)
        || app.ApplicantPerson.NationalNo == searchTerm
        || app.CreatedByUser.UserName == searchTerm);
    }

    public static IQueryable<Application> ApplyFilters(this IQueryable<Application> query, ApplicationsQueryParameters? parameters)
    {
        if (parameters == null)
            return query;

        if (parameters.ApplicationTypeID.HasValue)
            query = query.Where(app => app.ApplicationTypeID == parameters.ApplicationTypeID);

        if (parameters.ApplicationStatusID.HasValue)
            query = query.Where(app => app.ApplicationStatusID == parameters.ApplicationStatusID);

        if (parameters.DateFrom.HasValue)
            query = query.Where(app => app.ApplicationDate >= parameters.DateFrom);

        if (parameters.DateTo.HasValue)
            query = query.Where(app => app.ApplicationDate <= parameters.DateTo);

        return query;
    }

    public static IQueryable<Application> ApplySort(this IQueryable<Application> query, ApplicationsQueryParameters parameters)
    {
        bool descending = string.Equals(parameters.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return parameters.SortBy?.ToLower() switch
        {
            "applicationdate" =>
                descending
                    ? query.OrderByDescending(app => app.ApplicationDate)
                            .ThenByDescending(app => app.ApplicationID)
                    : query.OrderBy(app => app.ApplicationDate)
                            .ThenBy(app => app.ApplicationID),

            "applicationstatus" =>
                descending
                    ? query.OrderByDescending(app => app.ApplicationStatus)
                            .ThenByDescending(app => app.ApplicationID)

                    : query.OrderBy(app => app.ApplicationStatus)
                            .ThenBy(app => app.ApplicationID),

            "applicationtype" =>
                descending
                    ? query.OrderByDescending(p => p.ApplicationType)
                            .ThenByDescending(app => app.ApplicationID)

                    : query.OrderBy(app => app.ApplicationType)
                            .ThenBy(app => app.ApplicationID),

            _ =>
                query.OrderBy(app => app.ApplicationID)
        };
    }
}
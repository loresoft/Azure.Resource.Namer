using System.Security.Claims;

using AzureNamer.Shared.Definitions;
using AzureNamer.Shared.Extensions;

using LoreSoft.Blazor.Controls;

using MediatR.CommandQuery.Queries;

namespace AzureNamer.Client.Services;

public static class QueryBuilder
{
    public static EntityFilter? UserOrgainziations(ClaimsPrincipal principal)
    {
        var userId = principal.GetUserId();
        if (userId == null)
            return null;

        return new EntityFilter
        {
            Name = "UserId",
            Value = userId,
            Operator = EntityFilterOperators.Equal
        };
    }

    public static EntityQuery CreateQuery(
        DataRequest request,
        int? organizationId,
        string? searchText = null,
        IEnumerable<string>? searchFields = null,
        string? searchOperator = null)
    {
        ArgumentNullException.ThrowIfNull(request);

        var organizationFilter = OrganizationFilter(organizationId);
        var searchFilter = SearchFilter(searchText, searchFields, searchOperator);

        var filter = new EntityFilter
        {
            Logic = EntityFilterLogic.And,
            Filters = []
        };

        if (organizationFilter != null)
            filter.Filters.Add(organizationFilter);
        if (searchFilter != null)
            filter.Filters.Add(searchFilter);

        return new EntityQuery
        {
            Page = request.Page,
            PageSize = request.PageSize,
            Sort = CreateSort(request),
            Filter = filter
        };
    }


    public static EntityQuery CreateQuery(
        DataRequest request,
        string? searchText = null,
        IEnumerable<string>? searchFields = null,
        string? searchOperator = null)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new EntityQuery
        {
            Page = request.Page,
            PageSize = request.PageSize,
            Sort = CreateSort(request),
            Filter = SearchFilter(searchText, searchFields, searchOperator)
        };
    }

    public static EntityFilter? SearchFilter(
        string? searchText,
        IEnumerable<string>? searchFields,
        string? searchOperator = null)
    {
        if (searchText.IsNullOrWhiteSpace() || searchFields == null)
            return null;

        var textFilters = searchFields
            .Select(field => new EntityFilter
            {
                Name = field,
                Value = searchText,
                Operator = searchOperator ?? EntityFilterOperators.Contains
            })
            .ToList();

        return new EntityFilter
        {
            Logic = EntityFilterLogic.Or,
            Filters = textFilters
        };
    }

    public static EntityFilter? OrganizationFilter(int? organizationId)
    {
        if (!organizationId.HasValue)
            return null;

        return new EntityFilter
        {
            Name = nameof(IHaveOrganization.OrganizationId),
            Value = organizationId,
            Operator = EntityFilterOperators.Equal
        };
    }

    public static List<EntitySort>? CreateSort(DataRequest request)
    {
        if (request.Sorts == null || request.Sorts.Length == 0)
            return null;

        return request.Sorts
            .Select(s => new EntitySort
            {
                Name = s.Property,
                Direction = s.Descending ? "Desc" : "Asc"
            })
            .ToList();
    }
}

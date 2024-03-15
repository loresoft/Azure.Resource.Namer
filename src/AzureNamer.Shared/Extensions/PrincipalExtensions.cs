using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;

using AzureNamer.Shared.Constants;
using AzureNamer.Shared.Models;

namespace AzureNamer.Shared.Extensions;

public static class PrincipalExtensions
{
    public static int? GetUserId(this IPrincipal principal)
    {
        if (principal is null)
            throw new ArgumentNullException(nameof(principal));

        var claimPrincipal = principal as ClaimsPrincipal;
        var claim = claimPrincipal?.FindFirst(Claims.UserId);

        return int.TryParse(claim?.Value, out var uid) ? uid : null;
    }

    public static IReadOnlyCollection<OrganizationMembership>? GetOrganizations(this IPrincipal principal)
    {
        if (principal is null)
            throw new ArgumentNullException(nameof(principal));

        var claimPrincipal = principal as ClaimsPrincipal;
        var claim = claimPrincipal?.FindFirst(Claims.Organizations);

        if (claim == null || claim.Value.IsNullOrWhiteSpace())
            return null;

        return JsonSerializer.Deserialize(claim.Value, DomainJsonContext.Default.IReadOnlyCollectionOrganizationMembership);
    }

}

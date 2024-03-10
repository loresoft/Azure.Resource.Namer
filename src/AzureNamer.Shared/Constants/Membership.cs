using AzureNamer.Shared.Models;

namespace AzureNamer.Shared.Constants;

public static class Membership
{
    public static UserMembership AnonymousUser => new(false, 0, Guid.Empty, "Anonymous", string.Empty, false, [DefaultOrganization]);

    public static OrganizationMembership DefaultOrganization => new(1, "- default -", "org", true);
}

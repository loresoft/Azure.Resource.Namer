namespace AzureNamer.Shared.Models;

public record OrganizationMembership(
    int Id,
    string Name,
    string Abbreviation,
    bool IsLocked = false);

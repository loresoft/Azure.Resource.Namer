namespace AzureNamer.Shared.Models;

public class BrowserDetail
{
    public string? UserAgent { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public string? Platform { get; set; }

    public string? Version { get; set; }

    public string? Device { get; set; }

    public string? IpAddress { get; set; }

    public DateTimeOffset? Created { get; set; }
}

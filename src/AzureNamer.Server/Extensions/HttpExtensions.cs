using AzureNamer.Shared.Models;

using MyCSharp.HttpUserAgentParser;

namespace AzureNamer.Server.Extensions;

public static class HttpExtensions
{
    public static BrowserDetail? GetBrowserData(this HttpContext? httpContext)
    {
        return httpContext?.Request.GetBrowserData();
    }

    public static BrowserDetail? GetBrowserData(this HttpRequest? httpRequest)
    {
        if (httpRequest == null)
            return null;

        var model = new BrowserDetail { Created = DateTimeOffset.UtcNow };

        if (httpRequest == null)
            return model;

        model.IpAddress = httpRequest.HttpContext.Connection.RemoteIpAddress?.ToString();
        model.UserAgent = httpRequest.Headers.UserAgent.ToString();

        if (string.IsNullOrEmpty(model.UserAgent))
            return model;

        var userAgentInformation = HttpUserAgentParser.Parse(model.UserAgent);

        model.Name = userAgentInformation.Name;
        model.Type = userAgentInformation.Type.ToString();
        model.Platform = userAgentInformation.Platform.ToString();
        model.Version = userAgentInformation.Version;
        model.Device = userAgentInformation.MobileDeviceType;

        return model;
    }
}

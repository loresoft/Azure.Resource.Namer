using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AzureNamer.Core.Options;

public static class ConfigurationServiceModule
{
    [RegisterServices]
    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
    public static void Register(IServiceCollection services)
    {
        services
            .AddOptions<SendGridConfiguration>()
            .Configure<IConfiguration>((settings, configuration) => configuration
                .GetSection(SendGridConfiguration.ConfigurationName)
                .Bind(settings)
            );
    }
}

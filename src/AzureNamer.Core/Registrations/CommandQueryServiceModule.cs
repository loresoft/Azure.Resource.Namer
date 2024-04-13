using AzureNamer.Core.Data;
using AzureNamer.Shared;

using MediatR.CommandQuery;

using Microsoft.Extensions.DependencyInjection;

namespace AzureNamer.Core.Registrations;

public static class CommandQueryServiceModule
{
    [RegisterServices]
    public static void Register(IServiceCollection services)
    {
        services.AddMediator();
        services.AddAutoMapper(typeof(AzureNamerContext).Assembly, typeof(DomainJsonContext).Assembly);
    }
}

using System;
using System.Collections.Generic;
using MediatR.CommandQuery.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AzureNamer.Shared.Models;

// ReSharper disable once CheckNamespace
namespace AzureNamer.Core.Registrations;

public static class EnvironmentServiceRegistration
{
    [RegisterServices]
    public static void Register(IServiceCollection services)
    {
        #region Generated Register
        services.AddEntityQueries<AzureNamer.Core.Data.AzureNamerContext, AzureNamer.Core.Data.Entities.Environment, int, EnvironmentReadModel>();

        services.AddEntityCommands<AzureNamer.Core.Data.AzureNamerContext, AzureNamer.Core.Data.Entities.Environment, int, EnvironmentReadModel, EnvironmentCreateModel, EnvironmentUpdateModel>();

        #endregion
    }

}


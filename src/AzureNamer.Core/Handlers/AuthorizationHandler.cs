using AutoMapper;

using AzureNamer.Core.Commands;
using AzureNamer.Core.Data;
using AzureNamer.Core.Data.Entities;
using AzureNamer.Shared.Constants;
using AzureNamer.Shared.Extensions;
using AzureNamer.Shared.Models;

using MediatR.CommandQuery.EntityFrameworkCore.Handlers;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AzureNamer.Core.Handlers;

public class AuthorizationHandler : DataContextHandlerBase<AzureNamerContext, AuthorizationCommand, UserMembership>
{
    public AuthorizationHandler(ILoggerFactory loggerFactory, AzureNamerContext dataContext, IMapper mapper)
        : base(loggerFactory, dataContext, mapper)
    {
    }

    protected override async Task<UserMembership> Process(AuthorizationCommand request, CancellationToken cancellationToken)
    {
        if (request.Principal == null)
            return Membership.AnonymousUser;

        var objectId = request.Principal.GetObjectId();
        var name = request.Principal.GetName();
        var email = request.Principal.GetEmail();
        var provider = request.Principal.GetProvider();

        if (objectId == null)
            return Membership.AnonymousUser;

        var user = await DataContext.Users
            .Where(u => u.Identifier == objectId.Value)
            .FirstOrDefaultAsync();

        // create user if not found
        if (user == null)
        {
            user = new Data.Entities.User
            {
                Identifier = objectId.Value,
                Name = name ?? "Unknown",
                Email = email ?? "Unknown",
                Provider = provider
            };

            await DataContext.Users.AddAsync(user, cancellationToken);
            await DataContext.SaveChangesAsync(cancellationToken);
        }

        // save login request
        if (request.BrowserDetail != null)
        {
            var userLogin = new UserLogin
            {
                UserId = user.Id,
                Name = request.BrowserDetail.Name,
                Type = request.BrowserDetail.Type,
                Device = request.BrowserDetail.Device,
                Platform = request.BrowserDetail.Platform,
                Version = request.BrowserDetail.Version,
                IpAddress = request.BrowserDetail.IpAddress,
                UserAgent = request.BrowserDetail.UserAgent,
            };
            await DataContext.UserLogins.AddAsync(userLogin, cancellationToken);
            await DataContext.SaveChangesAsync(cancellationToken);
        }

        var organizations = await DataContext.UserOrganizations
            .AsNoTracking()
            .Where(o => o.UserId == user.Id)
            .Select(o => o.Organization)
            .ToListAsync(cancellationToken);

        var membership = organizations
            .OrderBy(o => o.Name)
            .Select(o => new OrganizationMembership(o.Id, o.Name, o.Abbreviation))
            .ToList();

        // everyone has access to default org
        membership.Insert(0, Membership.DefaultOrganization);

        return new UserMembership(
            request.Principal?.Identity?.IsAuthenticated ?? false,
            user.Id,
            user.Identifier,
            user.Name,
            user.Email,
            user.IsAdministrator,
            membership);
    }
}

using System.Security.Principal;

using AzureNamer.Shared.Models;

using MediatR.CommandQuery.Commands;

namespace AzureNamer.Core.Commands;


public class AuthorizationCommand : PrincipalCommandBase<UserMembership>
{
    public AuthorizationCommand(IPrincipal principal, BrowserDetail? browserDetail) : base(principal)
    {
        BrowserDetail = browserDetail;
    }

    public BrowserDetail? BrowserDetail { get; }
}

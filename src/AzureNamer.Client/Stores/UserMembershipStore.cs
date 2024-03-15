using AzureNamer.Shared.Constants;
using AzureNamer.Shared.Extensions;
using AzureNamer.Shared.Models;

using Blazone.Authentication.Extensions;

using Microsoft.AspNetCore.Components.Authorization;

namespace AzureNamer.Client.Stores;

[RegisterScoped]
public class UserMembershipStore : StateBase<UserMembership>, IDisposable
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public UserMembershipStore(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _authenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChangedAsync;
    }

    public async Task Load(bool force = false)
    {
        // don't load if already loaded
        if (!force && Model != null)
            return;

        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        Load(state);
    }

    public void Dispose()
    {
        _authenticationStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChangedAsync;
    }

    private async void OnAuthenticationStateChangedAsync(Task<AuthenticationState> task)
    {
        var state = await task;
        Load(state);
    }

    private void Load(AuthenticationState state)
    {
        var user = state.User;

        var userMembership = new UserMembership(
            user.Identity?.IsAuthenticated ?? false,
            user.GetUserId() ?? 0,
            user.GetObjectId() ?? Guid.Empty,
            user.GetName() ?? string.Empty,
            user.GetEmail() ?? string.Empty,
            user.IsInRole(Roles.Administrators),
            user.GetOrganizations());

        Set(userMembership);
    }
}

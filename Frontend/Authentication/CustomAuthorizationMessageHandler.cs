using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

namespace Frontend.Authentication;

public sealed class CustomAuthorizationMessageHandler(AuthenticationStateProvider authStateProvider) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var provider = (CustomAuthenticationStateProvider)authStateProvider;
        var authState = await provider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            var accessToken = await provider.GetAuthToken();
            if (!string.IsNullOrEmpty(accessToken!))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}

using System.Net.Http.Headers;

namespace Hungry_Hub_Mobile.Core.Helpers;

public class AuthenticatedHttpClientHandler : HttpClientHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Вземаме token от storage
        var token = await TokenStorage.GetAccessTokenAsync();

        // Ако има token, добавяме го в header-ite
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        // Изпращаме заявката
        return await base.SendAsync(request, cancellationToken);
    }
}
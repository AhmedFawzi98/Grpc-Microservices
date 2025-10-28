using Duende.IdentityModel.Client;
using ShoppingCartClient.Clients;

namespace ShoppingCartClient.Services;

public class TokenService(IHttpClientFactory httpClientFactory)
{
    private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();

    public async Task<string> GetTokensync()
    {
        var authClient = httpClientFactory.CreateClient("Auth");

        if (_cache.TryGetValue("Token", out string token))
        {
            return token;
        }

        var tokenEndpoint = string.Empty;
        if (_cache.TryGetValue("TokenEndpoint", out string cachedTokenEndpoint))
        {
            tokenEndpoint = cachedTokenEndpoint;
        }
        else
        {
            var discoveryDocument = await authClient.GetDiscoveryDocumentAsync();

            if (discoveryDocument == null || discoveryDocument.IsError)
            {
                Console.WriteLine("an error occured while fetching discovery document from identity server.");
                return string.Empty;
            }

            _cache.Add("TokenEndpoint", discoveryDocument.TokenEndpoint);

            tokenEndpoint = discoveryDocument.TokenEndpoint;
        }

        var clientCredintialsTokenRequest = new ClientCredentialsTokenRequest()
        {
            Address = tokenEndpoint,
            ClientId = "ShoppingCartClient",
            ClientSecret = "secret",
            Scope = "ShoppingCartAPI",
        };

        TokenResponse tokenResponse = await authClient.RequestClientCredentialsTokenAsync(clientCredintialsTokenRequest);
        if (tokenResponse.IsError)
        {
            Console.WriteLine("an error occured while requesting token from identity server.");
            return string.Empty;
        }

        _cache.Add("Token", tokenResponse.AccessToken);

        return tokenResponse.AccessToken;
    }
}

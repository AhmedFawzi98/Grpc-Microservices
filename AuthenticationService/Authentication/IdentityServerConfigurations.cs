using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;

namespace Authentication;

public static class IdentityServerConfigurations
{
    public static IEnumerable<Client> Clients =>
    [
        new Client()
        {
            ClientId = "ShoppingCartClient",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            AllowedScopes = [
                "ShoppingCartAPI"
            ],
            ClientSecrets = [
                new Secret("secret".Sha256())
            ]
        }
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new ApiScope("ShoppingCartAPI", "Shopping Cart API")
    ];

    public static IEnumerable<ApiResource> ApiResources =>
    [
    ];

    public static IEnumerable<IdentityResource> IdentityResources =>
    [
    ];

    public static IEnumerable<TestUser> TestUsers =>
    [
    ];

}

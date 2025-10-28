using Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddIdentityServer()
    .AddInMemoryClients(IdentityServerConfigurations.Clients)
    .AddInMemoryApiScopes(IdentityServerConfigurations.ApiScopes)
    .AddDeveloperSigningCredential();

var app = builder.Build();

app.UseIdentityServer();

app.Run();

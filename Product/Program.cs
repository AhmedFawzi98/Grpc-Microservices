using Microsoft.EntityFrameworkCore;
using Product.Data;
using Product.Data.Constants;
using Product.HostedServices;
using Product.Services;
using Shared.DbSeeding;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddDbContext<ProductContext>(options =>
{
    options.UseInMemoryDatabase(DatabaseConstants.ProductDatabaseName);
});

builder.Services.AddScoped<IDbSeeder, ProductDbSeeder>();
builder.Services.AddHostedService<DbSeeder>();

var app = builder.Build();

app.MapGrpcService<GreeterService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

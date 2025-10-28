using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Product.Data.Constants;
using Shared.DbSeeding;
using Shared.Interceptors;
using ShoppingCart.Data;
using ShoppingCart.HostedServices;
using ShoppingCart.Integration;
using System.Reflection;
using static Grpc.Discount.DiscountGrpcService;

namespace ShoppingCart.Extensions;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        services.AddGrpc(options =>
        {
            if (env.IsDevelopment())
                options.EnableDetailedErrors = true;

            options.Interceptors.Add<ServerGrpcExceptionsInterceptor>();
            options.Interceptors.Add<ServerLoggingInterceptor>();
        });

        AddAuth(services, configuration);

        AddGrpcTypedClients(services, configuration);
        
        AddDataAccessServices(services, configuration);

        AddApplicationServices(services, configuration);

        AddUtilitiesServices(services, configuration);

        return services;
    }

    private static void AddAuth(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "https://localhost:5005";
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = false
                };
            });

        services.AddAuthorization();
    }

    private static void AddDataAccessServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ShoppingCartDbContext>(options =>
        {
            options.UseInMemoryDatabase(DatabaseConstants.ShoppingCartDatabaseName);
        });

        services.AddScoped<IDbSeeder, ShoppingCartDbSeeder>();
        services.AddHostedService<DbInitalizer>();
    }

    private static void AddGrpcTypedClients(IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpcClient<DiscountGrpcServiceClient>(options =>
        {
            options.Address = new Uri("https://localhost:5003");
        });

    }

    private static void AddApplicationServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDiscountIntegrationService,  DiscountIntegrationService>();
    }

    private static void AddUtilitiesServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(options =>
        {
            options.AddMaps(Assembly.GetExecutingAssembly());
        });
    }
}

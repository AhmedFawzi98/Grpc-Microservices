
using Discount.Data;
using Shared.Interceptors;

namespace Discount.Extensions;

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

        services.AddSingleton<DiscountContext>();

        return services;
    }
}

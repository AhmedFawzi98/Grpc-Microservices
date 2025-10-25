using Product.Data;
using Shared.DbSeeding;

namespace Product.HostedServices;

public class DbSeeder(IServiceScopeFactory serviceScopeFactory) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();

        var seeder = scope.ServiceProvider.GetRequiredService<IDbSeeder>();
        await seeder.SeedAsync();
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

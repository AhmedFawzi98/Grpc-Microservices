using ShoppingCart.GrpcServices;

namespace ShoppingCart.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureWebApplication(this WebApplication app)
    {
        app.MapGrpcService<ShoppingCartService>();

        app.UseAuthentication();
        
        app.UseAuthorization();

        return app;
    }
}

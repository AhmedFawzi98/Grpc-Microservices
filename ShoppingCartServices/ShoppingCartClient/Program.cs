using Grpc.Core;
using Grpc.Product;
using Grpc.ShoppingCart;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Interceptors;
using ShoppingCartClient.Services;
using static Grpc.Core.Metadata;
using static Grpc.Product.ProductGrpcService;
using static Grpc.ShoppingCart.ShoppingCartGrpcService;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddGrpcClient<ProductGrpcServiceClient>(options =>
        {
            options.Address = new Uri("https://localhost:5001");
        });
        services.AddGrpcClient<ShoppingCartGrpcServiceClient>(options =>
        {
            options.Address = new Uri("https://localhost:5002");
        })
        .AddInterceptor<ClientLoggingInterceptor>(Grpc.Net.ClientFactory.InterceptorScope.Client);

        services.AddHttpClient("Auth", options =>
        {
            options.BaseAddress = new Uri("https://localhost:5005");
        });

        services.AddSingleton<ShoppingCartRunner>();
        services.AddSingleton<TokenService>();

        services.AddTransient<ClientLoggingInterceptor>();
    })
    .Build();

var shoppingCartRunner = host.Services.GetRequiredService<ShoppingCartRunner>();

var shoppingCart1 = await shoppingCartRunner.GetOrCreateShoppingCart("ahmed");
Console.WriteLine(shoppingCart1);

var shoppingCart2 = await shoppingCartRunner.GetOrCreateShoppingCart("nonexisting");
Console.WriteLine(shoppingCart2);


var areItemsAdded = await shoppingCartRunner.FetchProductAndAddItemsToShoppingCart(shoppingCart1);
if(areItemsAdded)
{
    var updatedShoppingCart1 = await shoppingCartRunner.GetOrCreateShoppingCart(shoppingCart1.Username); 
    Console.WriteLine($"shopping cart items are added successfully to shopping cart with username: {shoppingCart1.Username}");
    Console.WriteLine($"updated shopping cart details:\n {updatedShoppingCart1}");
}
else
{
    Console.WriteLine($"shopping cart items are not added to shopping cart with username: {shoppingCart1.Username}, operation failed");
}

Console.ReadLine();



public class ShoppingCartRunner(
    ProductGrpcServiceClient productGrpcServiceClient,
    ShoppingCartGrpcServiceClient shoppingCartGrpcServiceClient,
    TokenService tokenService)
{
    public async Task<ShoppingCart> GetOrCreateShoppingCart(string username)
    {

        Console.WriteLine("-------------- GetOrCreateShoppingCart starts ----------------");

        var token = await tokenService.GetTokensync();

        var headers = new Metadata()
        {
            new Entry("Authorization", $"Bearer {token}"),
        };

        ShoppingCart shoppingCart = default!;
        try
        {
            var getShoppingCartrequest = new GetShoppingCartRequest() { Username = username };  
            shoppingCart = await shoppingCartGrpcServiceClient.GetShoppingCartAsync(getShoppingCartrequest, headers);
        }
        catch (RpcException getShoppingCartRpcException)
        {
            if (getShoppingCartRpcException.StatusCode == StatusCode.NotFound)
            {
                Console.WriteLine($"-------- shopping cart with username = {username} doesn't exist, starting creating a new shopping cart for the user");

                try
                {
                    var addShoppingCartRequest = new AddShoppingCartRequest() { DiscountCode = "CODE_100", Username = username };
                    shoppingCart = await shoppingCartGrpcServiceClient.AddShoppingCartAsync(addShoppingCartRequest, headers);
                }
                catch (RpcException addShoppingCartRpcException)
                {
                    Console.WriteLine("-------- an exception occured while communicating with shopping cart service through gRPC call.");
                    throw;
                }
            }
            else
            {
                Console.WriteLine("-------- an exception occured while communicating with shopping cart service through gRPC call.");
                throw;
            }
        }

        Console.WriteLine("-------------- GetOrCreateShoppingCart ends ----------------");

        return shoppingCart;
    }

    public async Task<bool> FetchProductAndAddItemsToShoppingCart(ShoppingCart shoppingCart)
    {
        var token = await tokenService.GetTokensync();

        var headers = new Metadata()
        {
            new Entry("Authorization", $"Bearer {token}"),
        };

        using var asyncClientStreamingCall = shoppingCartGrpcServiceClient.AddItemIntoShoppingCart(headers);

        var getAllProductsRequest = new GetAllProductsRequest() { };
        using var asyncServerStreamingCall = productGrpcServiceClient.GetAllProducts(getAllProductsRequest, headers);

        await foreach (var product in asyncServerStreamingCall.ResponseStream.ReadAllAsync())
        {
            var addItemInShoppingCartRequest = new AddItemIntoShoppingCartRequest()
            {
                Username = shoppingCart.Username,
                DiscountCode = "CODE_100",
                ShoppingCartItem = new ShoppingCartItemToAdd()
                {
                    ProductId = product.Id,
                    Price = product.Price,
                    Color = product.Id % 2 == 0 ? "White" : "Black",
                    ProductName = product.Name,
                    Quantity = 1,
                },
            };

            await asyncClientStreamingCall.RequestStream.WriteAsync(addItemInShoppingCartRequest);
        }

        await asyncClientStreamingCall.RequestStream.CompleteAsync();

        var addItemIntoShoppingCartResponse = await asyncClientStreamingCall;

        return addItemIntoShoppingCartResponse.Sucess;
    }
}


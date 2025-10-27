using Grpc.Core;
using Grpc.Product;
using Grpc.ShoppingCart;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        });

        services.AddSingleton<ShoppingCartRunner>();
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



public class ShoppingCartRunner(ProductGrpcServiceClient productGrpcServiceClient, ShoppingCartGrpcServiceClient shoppingCartGrpcServiceClient)
{
    public async Task<ShoppingCart> GetOrCreateShoppingCart(string username)
    {
        Console.WriteLine("-------------- GetOrCreateShoppingCart starts ----------------");

        ShoppingCart shoppingCart = default!;
        try
        {
            var getShoppingCartrequest = new GetShoppingCartRequest() { Username = username };  
            shoppingCart = await shoppingCartGrpcServiceClient.GetShoppingCartAsync(getShoppingCartrequest);
        }
        catch (RpcException getShoppingCartRpcException)
        {
            if (getShoppingCartRpcException.StatusCode == StatusCode.NotFound)
            {
                Console.WriteLine($"-------- shopping cart with username = {username} doesn't exist, starting creating a new shopping cart for the user");

                try
                {
                    var addShoppingCartRequest = new AddShoppingCartRequest() { DiscountCode = "CODE_100", Username = username };
                    shoppingCart = await shoppingCartGrpcServiceClient.AddShoppingCartAsync(addShoppingCartRequest);
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
        var asyncClientStreamingCall = shoppingCartGrpcServiceClient.AddItemIntoShoppingCart();

        var getAllProductsRequest = new GetAllProductsRequest() { };
        var asyncServerStreamingCall = productGrpcServiceClient.GetAllProducts(getAllProductsRequest);

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
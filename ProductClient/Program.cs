using Grpc.Core;
using Grpc.Product;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddGrpcClient<ProductGrpcService.ProductGrpcServiceClient>(options =>
        {
            options.Address = new Uri("https://localhost:5001");
        });
        services.AddSingleton<ProductRunner>();
    })
    .Build();

var productRunner = host.Services.GetRequiredService<ProductRunner>();

await productRunner.GetProductAsync();

await productRunner.GetAllProductsAsync();

await productRunner.AddProductAsync();

await productRunner.UpdateProductAsync();

await productRunner.DeleteProductAsync();

await productRunner.GetAllProductsAsync(); //check

await productRunner.InsertBulkProductsAsync();

await productRunner.GetAllProductsAsync(); //check

public class ProductRunner
{
    private readonly ProductGrpcService.ProductGrpcServiceClient _client;

    public ProductRunner(ProductGrpcService.ProductGrpcServiceClient client)
    {
        _client = client;
    }

    public async Task GetProductAsync()
    {
        Console.WriteLine("------ GetProductAsync Start--------");
        var request = new GetProductRequest { Id = 1 };
        var response = await _client.GetProductAsync(request);
        Console.WriteLine(response);
        Console.WriteLine("------ GetProductAsync End--------");

        Console.ReadLine();
    }

    public async Task GetAllProductsAsync()
    {
        Console.WriteLine("------ GetAllProductsAsync Start--------");
        var request = new GetAllProductsRequest();
        using var asyncServerStreamingCall = _client.GetAllProducts(request);

        await foreach(var product in asyncServerStreamingCall.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine(product);
        }
        
        Console.WriteLine("------ GetAllProductsAsync End--------");

        Console.ReadLine();
    }

    public async Task AddProductAsync()
    {
        Console.WriteLine("------ AddProductAsync Start--------");
        var request = new AddProductRequest()
        {
            Name = "my new product 1",
            Description = "a new test product (1) to add to stock",
            Price = 14.5m,
        };

       var response = await _client.AddProductAsync(request);
       Console.WriteLine(response);
       Console.WriteLine("------ AddProductAsync End--------");

       Console.ReadLine();
    }

    public async Task UpdateProductAsync()
    {
        Console.WriteLine("------ UpdateProductAsync Start--------");
        var request = new UpdateProductRequest()
        {
            Id = 1,
            Name = "Wireless Mouse - updated",
            Description = "A comfortable, high-precision wireless mouse with long battery life - updated",
            Price = 55.99m,
        };

        var response = await _client.UpdateProductAsync(request);
        Console.WriteLine(response);
        Console.WriteLine("------ UpdateProductAsync End--------");

        Console.ReadLine();
    }

    public async Task DeleteProductAsync()
    {
        Console.WriteLine("------ DeleteProductAsync Start--------");
        var request = new DeleteProductRequest()
        {
            Id = 2,
        };

        var response = await _client.DeleteProductAsync(request);
        Console.WriteLine(response);
        Console.WriteLine("------ DeleteProductAsync End--------");

        Console.ReadLine();
    }

    public async Task InsertBulkProductsAsync()
    {
        Console.WriteLine("------ InsertBulkProductsAsync Start--------");

        var asyncClientStreamingCall = _client.InsertBulkProducts();

        var addProductRequests = new List<AddProductRequest>()
        {
            new AddProductRequest()
            {
                Name = "streaming product 1",
                Description = "a product(1) to stream to server",
                Price = 44.93m,
            },
            new AddProductRequest()
            {
                Name = "streaming product 2",
                Description = "a product(2) to stream to server",
                Price = 55.32m,
            },
            new AddProductRequest()
            {
                Name = "streaming product 3",
                Description = "a product(3) to stream to server",
                Price = 93.15m,
            },
        };

        foreach (var addProductRequest in addProductRequests)
        {
            await asyncClientStreamingCall.RequestStream.WriteAsync(addProductRequest);
        }

        await asyncClientStreamingCall.RequestStream.CompleteAsync();

        var response = await asyncClientStreamingCall;

        Console.WriteLine(response);
        Console.WriteLine("------ InsertBulkProductsAsync End--------");

        Console.ReadLine();
    }
}

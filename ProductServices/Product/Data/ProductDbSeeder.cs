using Microsoft.EntityFrameworkCore;
using Product.Enums;
using Shared.DbSeeding;
using ProductEntity = Product.Models.Product;

namespace Product.Data;

public class ProductDbSeeder(ProductDbContext productDbContext) : IDbSeeder
{
    public async Task SeedAsync()
    {
        await SeedProductsAsync();
        await productDbContext.SaveChangesAsync();
    }

    private async Task SeedProductsAsync()
    {
        if (await productDbContext.Products.AnyAsync())
            return;

        var products = new List<ProductEntity>
        {
            new ProductEntity
            {
                Name = "Wireless Mouse",
                Description = "A comfortable, high-precision wireless mouse with long battery life.",
                Price = 25.99m,
                ProductStatus = ProductStatus.Instock,
                CreatedDate = DateTime.UtcNow
            },
            new ProductEntity
            {
                Name = "Mechanical Keyboard",
                Description = "A durable mechanical keyboard with customizable RGB lighting.",
                Price = 79.99m,
                ProductStatus = ProductStatus.Instock,
                CreatedDate = DateTime.UtcNow
            },
            new ProductEntity
            {
                Name = "HD Monitor",
                Description = "24-inch Full HD monitor with thin bezels and vibrant colors.",
                Price = 149.99m,
                ProductStatus = ProductStatus.Instock,
                CreatedDate = DateTime.UtcNow
            },
            new ProductEntity
            {
                Name = "USB-C Docking Station",
                Description = "Multi-port docking station with HDMI, USB 3.0, and Ethernet support.",
                Price = 89.99m,
                ProductStatus = ProductStatus.Low,
                CreatedDate = DateTime.UtcNow
            },
            new ProductEntity
            {
                Name = "Noise Cancelling Headphones",
                Description = "Over-ear headphones with active noise cancellation and Bluetooth connectivity.",
                Price = 199.99m,
                ProductStatus = ProductStatus.None,
                CreatedDate = DateTime.UtcNow
            }
        };

        productDbContext.AddRange(products);
    }
}
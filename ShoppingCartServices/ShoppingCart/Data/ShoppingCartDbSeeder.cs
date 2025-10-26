using Microsoft.EntityFrameworkCore;
using Shared.DbSeeding;
using ShoppingCart.Data.Models;
using ShoppingCartEntity = ShoppingCart.Data.Models.ShoppingCart;

namespace ShoppingCart.Data;

public class ShoppingCartDbSeeder(ShoppingCartDbContext shoppingCartDbContext) : IDbSeeder
{
    public async Task SeedAsync()
    {
        await SeedProductsAsync();
        await shoppingCartDbContext.SaveChangesAsync();
    }

    private async Task SeedProductsAsync()
    {
        if (await shoppingCartDbContext.ShoppingCarts.AnyAsync())
            return;

        var shoppingCarts = new List<ShoppingCartEntity>
        {
            new ShoppingCartEntity
            {
                UserName = "ahmed",
                Items = new List<ShoppingCartItem>
                {
                    new ShoppingCartItem
                    {
                        ProductId = 1,
                        ProductName = "Wireless Mouse",
                        Quantity = 2,
                        Price = 25.99m,
                        Color = "Black"
                    },
                    new ShoppingCartItem
                    {
                        ProductId = 2,
                        ProductName = "Mechanical Keyboard",
                        Quantity = 1,
                        Price = 79.99m,
                        Color = "White"
                    }
                }
            },
            new ShoppingCartEntity
            {
                UserName = "john",
                Items = new List<ShoppingCartItem>
                {
                    new ShoppingCartItem
                    {
                        ProductId = 3,
                        ProductName = "HD Monitor",
                        Quantity = 1,
                        Price = 149.99m,
                        Color = "Black"
                    }
                }
            },
            new ShoppingCartEntity
            {
                UserName = "michael",
                Items = new List<ShoppingCartItem>
                {
                    new ShoppingCartItem
                    {
                        ProductId = 4,
                        ProductName = "USB-C Docking Station",
                        Quantity = 1,
                        Price = 89.99m,
                        Color = "Gray"
                    },
                    new ShoppingCartItem
                    {
                        ProductId = 5,
                        ProductName = "Noise Cancelling Headphones",
                        Quantity = 1,
                        Price = 199.99m,
                        Color = "Red"
                    }
                }
            }
        };

        shoppingCartDbContext.AddRange(shoppingCarts);
    }
}
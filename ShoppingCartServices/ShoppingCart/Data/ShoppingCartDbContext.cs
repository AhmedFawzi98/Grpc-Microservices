using Microsoft.EntityFrameworkCore;
using ShoppingCart.Data.Models;
using ShoppingCartEntity = ShoppingCart.Data.Models.ShoppingCart;

namespace ShoppingCart.Data;

public class ShoppingCartDbContext : DbContext
{
    public ShoppingCartDbContext(DbContextOptions options) : base(options)
    {
    }

    protected ShoppingCartDbContext()
    {
    }

    public DbSet<ShoppingCartEntity> ShoppingCarts { get; set; }

    public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

}


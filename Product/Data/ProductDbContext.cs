using Microsoft.EntityFrameworkCore;
using ProductEntity = Product.Models.Product;

namespace Product.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions options) : base(options)
    {
    }

    protected ProductDbContext()
    {
    }

    public DbSet<ProductEntity> Products { get; set; }

}


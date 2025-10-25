using Microsoft.EntityFrameworkCore;
using ProductEntity = Product.Models.Product;

namespace Product.Data;

public class ProductContext : DbContext
{
    public ProductContext(DbContextOptions options) : base(options)
    {
    }

    protected ProductContext()
    {
    }

    public DbSet<ProductEntity> Products { get; set; }

}


using Microsoft.EntityFrameworkCore;
using M12_HW.Models;

    public class ProductStoreContext : DbContext
    {
        public ProductStoreContext(DbContextOptions<ProductStoreContext> options) : base(options) { }


        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> CartItems { get; set; }
}
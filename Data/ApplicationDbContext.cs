using Confectionery.Models;
using Microsoft.EntityFrameworkCore;

namespace Confectionery.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<StatusOrder> StatusOrders { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<Catalog> Catalog { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BasketCatalog> BasketCatalogs { get; set; }
        public DbSet<OrderCatalog> OrderCatalogs { get; set; }
    }
}

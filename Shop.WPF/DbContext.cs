using Microsoft.EntityFrameworkCore;

namespace Shop.WPF
{
    class Context : DbContext
    {
        public DbSet<Product> Products { get; set; }
        //public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=products.db"); 
         //   optionsBuilder.UseLazyLoadingProxies();
        }
    }
}

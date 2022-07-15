using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Shop.WPF
{
    class ShopContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Shipping> Shippings { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=shop.db"); 
        }
    }

    public class Order
    {
        public int OrderId { get; set; }

        public int Quantity { get; set; }

        [NotMapped]
        public double Total => (Quantity * Product.Price) + Shipping.ShippingCost;

        public Product Product { get; set; }
        public Customer Customer { get; set; }
        public Shipping Shipping { get; set; }
    }

    public class Customer
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
    }

    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
    }  
    
    public class Shipping
    {
        public int ShippingId { get; set; }
        public string ShippingName { get; set; }
        public double ShippingCost { get; set; }
    }
}

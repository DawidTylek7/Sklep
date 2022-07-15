using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.WPF
{
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
}
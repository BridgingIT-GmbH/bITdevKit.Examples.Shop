namespace Domain.Shop.Model
{
    public class OrderDetail
    {
        public decimal UnitPrice { get; set; }

        public short Quantity { get; set; }

        public float Discount { get; set; }

        public Guid OrderId { get; set; } // not really needed

        public Order Order { get; set; }

        public Guid ProductId { get; set; } // not really needed

        public Product Product { get; set; }
    }
}

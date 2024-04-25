namespace Domain.Shop.Model
{
    public class Order
    {
        public Order() => this.OrderDetails = new HashSet<OrderDetail>();

        public Guid Id { get; set; }

        public DateTime? OrderedDate { get; set; }

        public DateTime? RequiredDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        public decimal? FreightCost { get; set; }

        public Address BillingAddress { get; set; }

        public Address ShippingAddress { get; set; }

        public Guid CustomerId { get; set; } // not really needed

        public Customer Customer { get; set; }

        public Guid? ShipperId { get; set; } // not really needed

        public Shipper Shipper { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; private set; }
    }
}

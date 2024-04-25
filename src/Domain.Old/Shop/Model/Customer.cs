namespace Domain.Shop.Model
{
    public class Customer
    {
        public Customer() => this.Orders = new HashSet<Order>();

        public Guid Id { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Title { get; set; }

        public string Phone { get; set; }

        public Address Address { get; set; }

        public ICollection<Order> Orders { get; private set; }
    }
}
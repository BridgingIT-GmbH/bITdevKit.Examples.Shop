namespace Domain.Shop.Model
{
    public class Shipper
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string HomePage { get; set; }

        public Address Address { get; set; }
    }
}

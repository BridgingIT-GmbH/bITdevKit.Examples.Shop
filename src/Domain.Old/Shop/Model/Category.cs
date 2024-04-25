namespace Domain.Shop.Model
{
    public class Category
    {
        public Category() => this.Products = new HashSet<Product>();

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public byte[] Picture { get; set; }

        public Guid? ParentId { get; set; } // not really needed

        public Category Parent { get; private set; }

        public ICollection<Product> Products { get; private set; }
    }
}

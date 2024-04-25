namespace Domain.Shop.Model
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string QuantityPerUnit { get; set; }

        public decimal? UnitPrice { get; set; }

        public short? UnitsInStock { get; set; }

        public short? UnitsOnOrder { get; set; }

        public short? ReorderLevel { get; set; }

        public bool Discontinued { get; set; }

        public Guid CategoryId { get; set; } // not really needed

        public Category Category { get; set; }

        public Guid SupplierId { get; set; } // not really needed

        public Supplier Supplier { get; set; }
    }
}

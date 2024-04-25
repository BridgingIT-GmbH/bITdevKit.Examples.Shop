namespace Domain.Shop.Model
{
    public class Region
    {
        public Region() => this.Territories = new HashSet<Territory>();

        public Guid Id { get; set; }

        public string Description { get; set; }

        public ICollection<Territory> Territories { get; private set; }
    }
}

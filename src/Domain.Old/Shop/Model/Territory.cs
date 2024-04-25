namespace Domain.Shop.Model
{
    public class Territory
    {
        public Territory() => this.Employees = new HashSet<Employee>();

        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid RegionId { get; set; } // not really needed

        public Region Region { get; set; }

        public ICollection<Employee> Employees { get; private set; }
    }
}

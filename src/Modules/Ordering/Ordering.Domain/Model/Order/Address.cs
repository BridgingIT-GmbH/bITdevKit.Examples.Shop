namespace Modules.Ordering.Domain;

using BridgingIT.DevKit.Domain.Model;

public class Address : ValueObject
{
    private Address()
    {
    }

    public string Line1 { get; init; }

    public string Line2 { get; init; }

    public string PostalCode { get; init; }

    public string City { get; init; }

    public string Country { get; init; }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return this.Line1;
        yield return this.Line2;
        yield return this.PostalCode;
        yield return this.City;
        yield return this.Country;
    }
}

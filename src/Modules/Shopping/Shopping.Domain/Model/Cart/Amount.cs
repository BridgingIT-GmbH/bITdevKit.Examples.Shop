namespace Modules.Shopping.Domain;

using BridgingIT.DevKit.Domain;
using BridgingIT.DevKit.Domain.Model;

public class Amount : DecimalValueObject
{
    private Amount() // TODO: make private again when System.Text.Json can deserialize objects with a non-public ctor
    {
    }

    private Amount(decimal value)
        : base(value)
    {
    }

    public static Amount Zero => new(0);

    public static implicit operator Amount(decimal value) => new(value);

    public static implicit operator decimal(Amount value) => value.Amount;

    public static Amount operator +(Amount a, Amount b) => a.Amount + b.Amount;

    public static Amount operator -(Amount a, Amount b) => a.Amount - b.Amount;

    public static Amount For(decimal value)
    {
        Check.Throw(new IBusinessRule[]
        {
            new IsValidPositiveAmount(value)
        });

        return new Amount(value);
    }
}
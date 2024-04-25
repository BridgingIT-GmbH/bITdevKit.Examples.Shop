namespace Modules.Shopping.Infrastructure.EntityFramework.Configurations.Converters;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modules.Shopping.Domain;

public class AmountConverter : ValueConverter<Amount, decimal>
{
    public AmountConverter()
        : base(
            v => v.Amount,
            v => Amount.For(v))
    {
    }
}
namespace Modules.Ordering.Domain;

using BridgingIT.DevKit.Domain.Model;
using BridgingIT.DevKit.Domain;

public class EmailAddress : ValueObject
{
    private EmailAddress() // TODO: make private again when System.Text.Json can deserialize objects with a non-public ctor
    {
    }

    private EmailAddress(string value)
    {
        EnsureArg.IsNotNullOrEmpty(value, nameof(value));

        this.Value = value;
        this.UserName = value[..value.IndexOf('@')];
        this.Domain = value[value.IndexOf('@')..].Trim('@');
    }

    public string UserName { get; }

    public string Domain { get; }

    public string Value { get; }

    public static implicit operator string(EmailAddress d) => d.Value;

    public static EmailAddress For(string value)
    {
        EnsureArg.IsNotNullOrEmpty(value, nameof(value));

        Check.Throw(new IBusinessRule[]
        {
            new IsValidEmailAddress(value)
        });

        return new EmailAddress(value);
    }

    public override string ToString() => $"{this.UserName}@{this.Domain}";

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return this.Domain;
        yield return this.UserName;
    }
}
namespace Modules.Shopping.Domain;

using BridgingIT.DevKit.Domain;

public class IsValidPositiveAmount : IBusinessRule
{
    private readonly decimal amount;

    public IsValidPositiveAmount(decimal amount)
    {
        this.amount = amount;
    }

    public string Message => "Not a valid positive amount";

    public Task<bool> IsSatisfiedAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(this.amount >= 0 && this.amount <= decimal.MaxValue);
}
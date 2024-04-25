namespace Modules.Shopping.Domain;

using BridgingIT.DevKit.Domain;

public class IsValidCartProduct : IBusinessRule
{
    private readonly CartProduct product;

    public IsValidCartProduct(CartProduct product)
    {
        this.product = product;
    }

    public string Message => "Not a valid cart product";

    public Task<bool> IsSatisfiedAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(!string.IsNullOrEmpty(this.product.SKU)
            && !string.IsNullOrEmpty(this.product.Name)
            && this.product.UnitPrice > Amount.Zero);
}
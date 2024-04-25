namespace Modules.Shopping.Application.Queries;

using System.Threading;
using System.Threading.Tasks;
using BridgingIT.DevKit.Application.Queries;
using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Domain.Repositories;
using BridgingIT.DevKit.Domain.Specifications;
using Microsoft.Extensions.Logging;
using Modules.Shopping.Domain;

public class CartFindOneQueryHandler : QueryHandlerBase<CartFindOneQuery, Result<Cart>>
{
    private readonly IGenericRepository<Cart> repository;

    public CartFindOneQueryHandler(ILoggerFactory loggerFactory, IGenericRepository<Cart> repository)
        : base(loggerFactory)
    {
        EnsureArg.IsNotNull(repository, nameof(repository));

        this.repository = repository;
    }

    public override async Task<QueryResponse<Result<Cart>>> Process(CartFindOneQuery query, CancellationToken cancellationToken)
    {
        var entity = (await this.repository.FindAllAsync(
            specification: new Specification<Cart>(e => e.Identity == query.Identity),
            cancellationToken: cancellationToken).AnyContext()).FirstOrDefault();

        if (entity == null)
        {
            entity = Cart.ForUser(query.Identity);
            await this.repository.InsertAsync(entity, cancellationToken);
        }

        return new QueryResponse<Result<Cart>>()
        {
            Result = Result<Cart>.Success(entity)
        };
    }
}
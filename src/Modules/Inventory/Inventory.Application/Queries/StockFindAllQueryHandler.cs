namespace Modules.Inventory.Application;

using BridgingIT.DevKit.Application.Queries;
using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Modules.Inventory.Domain;

public class StockFindAllQueryHandler : QueryHandlerBase<StockFindAllQuery, PagedResult<Stock>>
{
    private readonly IGenericRepository<Stock> repository;

    public StockFindAllQueryHandler(ILoggerFactory loggerFactory, IGenericRepository<Stock> repository)
        : base(loggerFactory)
    {
        EnsureArg.IsNotNull(repository, nameof(repository));

        this.repository = repository;
    }

    public override async Task<QueryResponse<PagedResult<Stock>>> Process(StockFindAllQuery query, CancellationToken cancellationToken)
    {
        // TODO: replace with new repo.FindAllPagedResultAsync(....)
        var count = await this.repository.CountAsync(cancellationToken: cancellationToken).AnyContext();

        var entities = await this.repository.FindAllAsync(
            options: new FindOptions<Stock>
            {
                Skip = (query.PageNumber - 1) * query.PageSize,
                Take = query.PageSize,
            },
            cancellationToken: cancellationToken).AnyContext();

        return new QueryResponse<PagedResult<Stock>>()
        {
            Result = PagedResult<Stock>.Success(entities, count, query.PageNumber, query.PageSize)
        };
    }
}
namespace Modules.Catalog.Infrastructure;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using BridgingIT.DevKit.Common;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Application;
using Modules.Catalog.Domain;

public class DummyJsonCatalogDataAdapter : ICatalogDataAdapter
{
    private readonly ILogger logger;
    private readonly HttpClient client;

    public DummyJsonCatalogDataAdapter(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory)
    {
        EnsureArg.IsNotNull(loggerFactory, nameof(loggerFactory));
        EnsureArg.IsNotNull(httpClientFactory, nameof(httpClientFactory));

        this.logger = loggerFactory.CreateLogger(this.GetType());
        this.client = httpClientFactory.CreateClient("DummyJsonClient");
    }

    public async IAsyncEnumerable<Brand> GetBrands([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // https://dummyjson.com/products
        var url = string.Empty;
        this.logger.LogDebug("dummyjson adapter: request (url={url})", url);

        var response = await this.client.GetAsync(url, cancellationToken).AnyContext();
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsAsync<IEnumerable<DummyJsonProduct>>(cancellationToken).AnyContext();
            this.logger.LogDebug("dummjson adapter: processed (count=#{DummyJsonProductCount})", content?.Count());

            foreach (var item in content.SafeNull().DistinctBy(p => p.Brand.SafeNull().ToLowerInvariant()))
            {
                yield return new Brand
                {
                    Id = GuidGenerator.Create(item.Brand.SafeNull().ToLowerInvariant()), // create repeatable id
                    Name = item.Brand,
                    Description = item.Description,
                };
            }
        }
        else
        {
            this.logger.LogError("dummyjson adapter: failed (status={StatusCode})", (int)response?.StatusCode);
            //    throw?
        }
    }

    public async IAsyncEnumerable<Product> GetProducts([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // https://dummyjson.com/products
        var url = string.Empty;
        this.logger.LogDebug("dummyjson adapter: request (url={url})", url);

        var response = await this.client.GetAsync(url, cancellationToken).AnyContext();
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsAsync<IEnumerable<DummyJsonProduct>>(cancellationToken).AnyContext();
            this.logger.LogDebug("dummjson adapter: processed (count=#{DummyJsonProductCount})", content?.Count());

            foreach (var item in content.SafeNull())
            {
                yield return new Product
                {
                    Id = GuidGenerator.Create(item.Id.ToString()), // create repeatable id
                    BrandId = GuidGenerator.Create(item.Brand.SafeNull().ToLowerInvariant()), // create repeatable id
                    TypeId = Guid.Parse("fea89522-888d-4e50-8381-3b7f550dfc01"),
                    Sku = GuidGenerator.Create($"{item.Title}_{item.Brand}".ToLowerInvariant()).ToString("N")[..13].ToUpper(), // create repeatable sku,
                    Barcode = GuidGenerator.Create($"{item.Title}_{item.Brand}".ToLowerInvariant()).ToString("N")[..13].ToUpper(), // create repeatable barcode,
                    Name = item.Title,
                    // product.category
                    // product.images[]
                    Description = $"{item.Description} [{item.Category}]",
                    Price = item.Price,
                    Rating = item.Rating.To<int>()
                };
            }
        }
        else
        {
            this.logger.LogError("dummyjson adapter: failed (status={StatusCode})", (int)response?.StatusCode);
            //    throw?
        }
    }
}
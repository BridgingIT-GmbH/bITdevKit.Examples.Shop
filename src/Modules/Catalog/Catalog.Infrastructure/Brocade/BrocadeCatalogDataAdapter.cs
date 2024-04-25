namespace Modules.Catalog.Infrastructure;

using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using BridgingIT.DevKit.Common;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Application;
using Modules.Catalog.Domain;

public class BrocadeCatalogDataAdapter : ICatalogDataAdapter
{
    private readonly ILogger logger;
    private readonly HttpClient client;
    private readonly string apiKey; // TODO: remove this

    public BrocadeCatalogDataAdapter(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, string apiKey)
    {
        EnsureArg.IsNotNull(loggerFactory, nameof(loggerFactory));
        EnsureArg.IsNotNull(httpClientFactory, nameof(httpClientFactory));
        EnsureArg.IsNotNullOrEmpty(apiKey, nameof(apiKey));

        this.logger = loggerFactory.CreateLogger(this.GetType());
        this.client = httpClientFactory.CreateClient("BrocadeClient");
        this.apiKey = apiKey;
    }

    public async IAsyncEnumerable<Brand> GetBrands([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.Delay(0, cancellationToken);
        yield return Enumerable.Empty<Brand>().FirstOrDefault();
    }

    public async IAsyncEnumerable<Product> GetProducts([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // http://www.brocade.io/
        const int page = 20;
        var url = $"api/items?page={page}";
        this.logger.LogInformation("brocade adapter: processing (page={BrocadePage})", page);
        this.logger.LogDebug("brocade adapter: request (url={url})", url);
        var response = await this.client.GetAsync(url, cancellationToken).AnyContext();
        if (response.IsSuccessStatusCode)
        {
            this.logger.LogInformation("brocade adapter: processed (page={BrocadePage})", page);
            var content = await response.Content.ReadAsAsync<IEnumerable<BrocadeProduct>>(cancellationToken).AnyContext();

            foreach (var item in content.SafeNull())
            {
                // TODO: let a fluent validator check for the following props
                if (string.IsNullOrEmpty(item.Gtin14))
                {
                    this.logger.LogWarning("brocade adapter: skip product (reason=gtin14)");
                    continue;
                }

                if (string.IsNullOrEmpty(item.Name))
                {
                    this.logger.LogWarning("brocade adapter: skip product (reason=name)");
                    continue;
                }

                if (string.IsNullOrEmpty(item.BrandName))
                {
                    this.logger.LogWarning("brocade adapter: skip product (reason=brandname)");
                    continue;
                }

                yield return new Product
                {
                    Id = GuidGenerator.Create(item.Gtin14.SafeNull().ToLowerInvariant()), // create repeatable id
                    BrandId = GuidGenerator.Create(item.BrandName.SafeNull().ToLowerInvariant()), // create repeatable id
                    Size = item.Size,
                    Name = item.Name,
                    Sku = item.Gtin14,
                    Barcode = item.Gtin14,
                    Rating = 3
                };
            }
        }
        else
        {
            this.logger.LogError("brocade adapter: failed (status={StatusCode}, page={BrocadePage})", (int)response?.StatusCode, page);
            //    throw?
        }
    }
}

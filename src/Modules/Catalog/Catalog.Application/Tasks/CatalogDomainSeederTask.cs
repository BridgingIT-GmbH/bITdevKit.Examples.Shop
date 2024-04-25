namespace Modules.Catalog.Application;

using System.Threading.Tasks;
using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Modules.Catalog.Domain;

public class CatalogDomainSeederTask : IStartupTask // TODO: move to domain layer?
{
    private readonly ILogger<CatalogDomainSeederTask> logger;
    private readonly IGenericRepository<Brand> brandRepository;
    private readonly IGenericRepository<ProductType> productTypeRepository;
    private readonly IGenericRepository<Product> productRepository;

    public CatalogDomainSeederTask(
        ILoggerFactory loggerFactory,
        IGenericRepository<Brand> brandRepository,
        IGenericRepository<ProductType> productTypeRepository,
        IGenericRepository<Product> productRepository)
    {
        this.logger = loggerFactory?.CreateLogger<CatalogDomainSeederTask>() ?? NullLoggerFactory.Instance.CreateLogger<CatalogDomainSeederTask>();
        this.brandRepository = brandRepository;
        this.productTypeRepository = productTypeRepository;
        this.productRepository = productRepository;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await this.SeedBrands(this.brandRepository);
        await this.SeedProductTypes(this.productTypeRepository);
        await this.SeedProducts(this.productRepository);
    }

    private async Task SeedBrands(IGenericRepository<Brand> repository)
    {
        if (!await repository.ExistsAsync(SeedIds.Brand.Brand01))
        {
            var entity = new Brand
            {
                Id = SeedIds.Brand.Brand01.To<Guid>(),
                Name = nameof(SeedIds.Brand.Brand01),
                Description = " Ut dolor voluptua labore sit duo eu et amet nulla rebum eirmod vulputate facilisi."
                //PictureSvg = BrandLogoSvg.Brand01
            };
            entity.AuditState.SetCreated("seed");
            await repository.InsertAsync(entity);
        }

        if (!await repository.ExistsAsync(SeedIds.Brand.Brand02))
        {
            var entity = new Brand
            {
                Id = SeedIds.Brand.Brand02.To<Guid>(),
                Name = nameof(SeedIds.Brand.Brand02),
                Description = " Ut dolor voluptua labore sit duo eu et amet nulla rebum eirmod vulputate facilisi."
                //PictureSvg = BrandLogoSvg.Brand02
            };
            entity.AuditState.SetCreated("seed");
            await repository.InsertAsync(entity);
        }
    }

    private async Task SeedProductTypes(IGenericRepository<ProductType> repository)
    {
        if (!await repository.ExistsAsync(SeedIds.Product.Product01))
        {
            var entity = new ProductType
            {
                Id = SeedIds.ProductType.ProductType01.To<Guid>(),
                Name = nameof(SeedIds.ProductType.ProductType01),
                Description = " Ut dolor voluptua labore sit duo eu et amet nulla rebum eirmod vulputate facilisi."
            };
            await repository.InsertAsync(entity);
        }
    }

    private async Task SeedProducts(IGenericRepository<Product> repository)
    {
        if (!await repository.ExistsAsync(SeedIds.Product.Product01))
        {
            var entity = new Product
            {
                Id = SeedIds.Product.Product01.To<Guid>(),
                TypeId = SeedIds.ProductType.ProductType01.To<Guid>(),
                BrandId = SeedIds.Brand.Brand01.To<Guid>(),
                Name = nameof(SeedIds.Product.Product01),
                Barcode = "ABC-abc-1231",
                Sku = "10074887611231",
                Description = " Ut dolor voluptua labore sit duo eu et amet nulla rebum eirmod vulputate facilisi.",
                Price = 9.99M,
                Size = "6.65 oz (151 g)",
                Rating = 3
                //PictureSvg = ProductLogoSvg.Product01
            };
            entity.AuditState.SetCreated("seed");
            await repository.InsertAsync(entity);
        }

        if (!await repository.ExistsAsync(SeedIds.Product.Product02))
        {
            var entity = new Product
            {
                Id = SeedIds.Product.Product02.To<Guid>(),
                TypeId = SeedIds.ProductType.ProductType01.To<Guid>(),
                BrandId = SeedIds.Brand.Brand01.To<Guid>(),
                Name = nameof(SeedIds.Product.Product02),
                Barcode = "ABC-abc-1232",
                Sku = "10074887611232",
                Description = " Ut dolor voluptua labore sit duo eu et amet nulla rebum eirmod vulputate facilisi.",
                Price = 7.99M,
                Size = "7.68 oz (189 g)",
                Rating = 4
                //PictureSvg = ProductLogoSvg.Product02
            };
            entity.AuditState.SetCreated("seed");
            await repository.InsertAsync(entity);
        }

        if (!await repository.ExistsAsync(SeedIds.Product.Product03))
        {
            var entity = new Product
            {
                Id = SeedIds.Product.Product03.To<Guid>(),
                TypeId = SeedIds.ProductType.ProductType01.To<Guid>(),
                BrandId = SeedIds.Brand.Brand01.To<Guid>(),
                Name = nameof(SeedIds.Product.Product03),
                Barcode = "ABC-abc-1233",
                Sku = "10074887611233",
                Description = " Ut dolor voluptua labore sit duo eu et amet nulla rebum eirmod vulputate facilisi.",
                Price = 8.89M,
                Size = "2.68 oz (111 g)",
                Rating = 5
                //PictureSvg = ProductLogoSvg.Product03
            };
            entity.AuditState.SetCreated("seed");
            await repository.InsertAsync(entity);
        }

        if (!await repository.ExistsAsync(SeedIds.Product.Product04))
        {
            var entity = new Product
            {
                Id = SeedIds.Product.Product04.To<Guid>(),
                TypeId = SeedIds.ProductType.ProductType01.To<Guid>(),
                BrandId = SeedIds.Brand.Brand01.To<Guid>(),
                Name = nameof(SeedIds.Product.Product04),
                Barcode = "ABC-abc-1234",
                Sku = "10074887611234",
                Description = " Ut dolor voluptua labore sit duo eu et amet nulla rebum eirmod vulputate facilisi.",
                Price = 12.99M,
                Size = "5.68 oz (161 g)",
                Rating = 2
                //PictureSvg = ProductLogoSvg.Product04
            };
            entity.AuditState.SetCreated("seed");
            await repository.InsertAsync(entity);
        }
    }

    private static class SeedIds
    {
        public struct Brand
        {
            public const string Brand01 = "ac51c174-e416-4313-9785-02b9a3117e01";
            public const string Brand02 = "ac51c174-e416-4313-9785-02b9a3117e02";
        }

        public struct ProductType
        {
            public const string ProductType01 = "fea89522-888d-4e50-8381-3b7f550dfc01";
        }

        public struct Product
        {
            public const string Product01 = "fd52219c-8e81-4920-9ffe-47f4d412d8a1";
            public const string Product02 = "fd52219c-8e81-4920-9ffe-47f4d412d8a2";
            public const string Product03 = "fd52219c-8e81-4920-9ffe-47f4d412d8a3";
            public const string Product04 = "fd52219c-8e81-4920-9ffe-47f4d412d8a4";
            public const string Product05 = "837b31e0-1610-4e56-bb8e-1d7c49b2f081";
            public const string Product06 = "837b31e0-1610-4e56-bb8e-1d7c49b2f082";
            public const string Product07 = "837b31e0-1610-4e56-bb8e-1d7c49b2f083";
        }
    }
}
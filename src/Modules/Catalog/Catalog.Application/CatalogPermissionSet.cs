namespace Modules.Catalog.Application;

using Common;

public class CatalogPermissionSet : PermissionSetBase
{
    [DisplayName("Products")]
    [Description("Products Permissions")]
    public static class Products
    {
        public const string View = "Catalog.Products.View";
        public const string Create = "Catalog.Products.Create";
        public const string Edit = "Catalog.Products.Edit";
        public const string Delete = "Catalog.Products.Delete";
        public const string Export = "Catalog.Products.Export";
        public const string Search = "Catalog.Products.Search";
        public const string Import = "Catalog.Products.Import";
    }

    [DisplayName("Brands")]
    [Description("Brands Permissions")]
    public static class Brands
    {
        public const string View = "Catalog.Brands.View";
        public const string Create = "Catalog.Brands.Create";
        public const string Edit = "Catalog.Brands.Edit";
        public const string Delete = "Catalog.Brands.Delete";
        public const string Export = "Catalog.Brands.Export";
        public const string Search = "Catalog.Brands.Search";
        public const string Import = "Catalog.Brands.Import";
    }
}
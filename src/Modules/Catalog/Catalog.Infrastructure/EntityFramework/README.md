These database commands should be executed from the solution root folder.

### new migration: 
- `dotnet ef migrations add Initial --context CatalogDbContext --output-dir .\EntityFramework\Migrations --project .\src\Modules\Catalog\Catalog.Infrastructure\Catalog.Infrastructure.csproj --startup-project .\src\Presentation.Web\Presentation.Web.csproj`

### update database: 
- `dotnet ef database update --project .\src\Modules\Catalog\Catalog.Infrastructure\Catalog.Infrastructure.csproj --startup-project .\src\Presentation.Web\Presentation.Web.csproj`

### create migrations bundle:

- `dotnet ef migrations bundle --self-contained -r linux-x64  --project .\src\Modules\Catalog\Catalog.Infrastructure\Catalog.Infrastructure.csproj --startup-project .\src\Presentation.Web\Presentation.Web.csproj --context CatalogDbContext --output .\efbundle-catalog --force`
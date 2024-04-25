#### TODO Q2_2022

- DONE: jwt token refresh
- DONE: favicon https://www.iconsdb.com/white-icons/shopping-cart-icon.html
- DONE: signalr setup
- DONE: logout user when deactivated (signalr like changerolepermissions)
- DONE: user > view profile > delete account
- client global exception handler > snackbar + logger
- DONE: get hotreload working
- DONE: minimal program/startup?
- localize backend (api > service)
- DONE: product/brand domain/infra/app/presentation + repo + > mediatr cmd/hndlr
- DONE: product import (like brands)
- profile pictures
- product translations (name/description) (like VGB designations )
- product pictures
- tenants
- timezone profile setting (+localstorage) > ClientPreferenceManager
- api umstellen auf ProblemDetails (statt result:succeeded=false)
- azure ci/cd setup

-----

#### TODO 2023

- DONE [FRAMEWORK] cleanup transitive dependencies (snitch) https://gsferreira.com/archive/2022/finding-dotnet-transitive-dependencies-and-tidying-up-your-project/

- DONE [FRAMEWORK] add jobscheduling behaviors in `ScopedJobFactory:ScopedJob:Execute()`

  - `pipeline > next` pattern, see messaging
  - so we have job pre/post start hooks (like messaging/commands/queries)
  - implement a `ChaosExceptionJobBehavior`
  - implement a `ModuleScopeJobBehavior` to set the module log context
  - maybe implement a simple job progress/status system with persistency (+UI)

- DONE [FRAMEWORK] MessageBrokerBase: propagate correlationid to the messagehandlers (+ set log scope with corr property in Process())

- [FRAMEWORK] add pulsar messaging monitoring > log

  - use `StateChangedHandler()` https://github.com/apache/pulsar-dotpulsar/wiki/Monitoring
  - consumer & producer

- [FRAMEWORK] add pulsar messaging exceptionhandling > log

  - use `ExceptionHandler()` https://github.com/apache/pulsar-dotpulsar/wiki/Resilience

- DONE [FRAMEWORK] setup dependabot dependency PRs

  - https://techcommunity.microsoft.com/t5/azure-devops-blog/keeping-your-dependencies-updated-with-azure-pipelines-and/ba-p/3590020
  - use [tinglesoft docker image](https://github.com/tinglesoftware/dependabot-azure-devops/tree/main/script) > because it has DEPENDABOT_OPEN_PULL_REQUESTS_LIMIT (0=security updates only)
  - OR with extension https://dev.azure.com/bitdevelopment/_settings/extensions?tab=requested&status=pending

- [FRAMEWORK] cross module synchronous communication (like inflow/[DevMentors](https://devmentors.io/me/courses/modular-monolith))

  - Module Requests
  - some module needs data from another module > no direct references!

- [FRAMEWORK] Queueing

  - needed later to put in orders
  - inprocess + pulsar > https://pulsar.apache.org/docs/cookbooks-message-queue/

- [FRAMEWORK] FileStorage

  - needed later for profile/product images to be stored

- [FRAMEWORK] TypedIds (for entities)
  - DONE provide a base typeidfor guid/int/long/string
  - Create a sourcegenerator which generates the full typeid based on an attribute, like [StronglyTypedId](https://github.dev/andrewlock/StronglyTypedId/blob/3abb1cd837a3fd33c52cccaddb6fbfe8660c810d/src/StronglyTypedIds/StronglyTypedIdGenerator.cs#L20)

- [FRAMEWORK] EntityCommands/Queries
  - DONE send entityCREATED/UPDATED/DELETEDcommand messages (through Messaging)
  - create dotnet templates for entitycommands/entityqueries
    - https://stackoverflow.com/questions/43615418/how-to-install-templates-from-nuget-package

- DONE [FRAMEWORK] + [SHOP] Convert to central package management https://bartwullems.blogspot.com/2022/12/convert-project-to-use-centralised.html & https://learn.microsoft.com/en-us/nuget/consume-packages/Central-Package-Management

- - [FRAMEWORK] create dotnet templates to generate module projects (app/domain/infra/pres)
  - https://damienbod.com/2022/08/15/creating-dotnet-solution-and-project-templates/
  - create multiple projects at once?

- [FRAMEWORK] add new DocumentRepository<TEntity> with a sqlserver IDocumentProvider to easily store entities/aggregates without EF/Migrations

  - use indexes for queryies, implements IGenericRepository

- handle robots.txt requests per environment (and prevent 404 errors in appinsights)

  - https://khalidabuhakmeh.com/robotstxt-middleware-aspnet-core
  - however /robots933456.txt seems a needed request from azure appservice https://github.com/MicrosoftDocs/azure-docs/issues/32472

- replace EPPLUS code in Common.Services due to licensing issue https://www.epplussoftware.com/en/LicenseOverview

  - https://github.com/blazorhero/CleanArchitecture/issues/330
  - OR use CsvHelper + CsvHelper.Excel to create/read excel files  https://joshclose.github.io/CsvHelper/
  - remove EPPLUS package reference

- DONE provide some standard CRUD commands/queries + handlers

  - see Application.Queries (EntityFindAllQueryBase)

- Blazor client use serilog + (optional) log ingest on server

  - https://nblumhardt.com/2019/11/serilog-blazor/
  - https://stackoverflow.com/questions/71220619/use-serilog-as-logging-provider-in-blazor-webassembly-client-app

- Azure pipelines

  - use the `az CLI` task instead of powershell https://github.com/william-liebenberg/bicep-deploy-webapp-managed-identities/blob/main/.azure/pipelines/templates/deploy-infra-steps.yml#L10

- DONE Bicep restructure

  - many samples https://github.com/Azure/azure-docs-bicep-samples/tree/main/samples
  - investigate AZD https://learn.microsoft.com/en-us/azure/developer/azure-developer-cli/azd-templates?tabs=nodejs
  - add cosmosdb bicep modules https://github.dev/Azure-Samples/todo-csharp-cosmos-sql
  - improve health check powershell task https://stackoverflow.com/a/63037309/1758814
  - try deploymentscript task https://github.com/Azure/azure-docs-bicep-samples/blob/main/samples/deployment-script/externalScript.bicep
  - try managed identity
    - https://learn.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/overview
    - https://github.com/Azure/azure-docs-bicep-samples/tree/main/samples/scenarios-rbac
    - https://jfarrell.net/2022/01/17/secure-configuration-in-azure-with-managed-identity/
  - DONE use abbreviations for azure resources https://github.dev/Azure-Samples/todo-csharp-cosmos-sql/blob/7ff3cab508f217fb377f82286566137f162d2ba9/infra/abbreviations.json#L108
  - DONE move to .azure root folder (also the yaml pipelines) https://github.com/william-liebenberg/bicep-deploy-webapp-managed-identities/tree/main/.azure
  - DONE tags to object https://github.com/william-liebenberg/bicep-deploy-webapp-managed-identities/blob/main/.azure/bicep/main.bicep#L26
  - DONE environment config map https://gregorsuttie.com/2022/03/30/config-mapping-in-bicep-cool-stuff/
    - DONE OR parameter files https://github.com/william-liebenberg/bicep-deploy-webapp-managed-identities/blob/main/.azure/main.parameters.dev.json
  - DONE azure managed identites https://github.com/william-liebenberg/bicep-deploy-webapp-managed-identities
    - DONE add keyvault https://github.com/william-liebenberg/bicep-deploy-webapp-managed-identities/blob/main/.azure/bicep/main.bicep#L78
    - DONE use more modules for storage/sql/web
      - https://github.com/william-liebenberg/bicep-deploy-webapp-managed-identities/blob/main/.azure/bicep/main.bicep#L90
      - https://github.com/william-liebenberg/bicep-deploy-webapp-managed-identities/blob/main/.azure/bicep/main.bicep#L102
    - DONE keyvault + webapp managed identity (access policy) SSW TV min 46:25 https://www.youtube.com/watch?v=gv_SFEBZ1WU
      - https://github.com/william-liebenberg/bicep-deploy-webapp-managed-identities/blob/main/.azure/bicep/modules/webApp.bicep#L123

- DONE appinsight serilog setup (appsettings + bicep)

- DONE configuration (appsettings) for all modules

- DONE module configuration in azure webapp (env settings)

- DONE put client (blazor) code/razor in the modules 

  - `src\Presentation.Web\Modules\Catalog` > `src\Modules\Catalog\Catalog.Presentation.Web.Client`

- DONE generate swagger + c# apiclient on BuildEvent

  - https://github.com/william-liebenberg/BlazorWasmHostedNet6NSwag

- DONE more modular swagger + client generation (not possible?)

  - an apiclient for each module, which only contains module controllers > actions
  - https://github.com/RicoSuter/NSwag/issues/2584
  - how to generate the c# clients based on the c# controllers instead of hosted swagger, then filtering is possible (operationprocessors) https://github.com/RicoSuter/NSwag/issues/3188
  - OR swap nswag with refit > https://code-maze.com/using-refit-to-consume-apis-in-csharp/

- Azure deploy Pulsar as a docker container (like SEQ) > bicep

- Azure deploy Mailhog as a docker container (like SEQ) > bicep

- Architecture Fitness tests > https://github.com/BenMorris/NetArchTest

  - DONE test that module boundaries are not crossed (see Hte)
  - DONE test that layers & dependencies inside modules are correct (see Zeta)
  - create reusable profiles, move profiles to bitdevkit
  - investigate [NsDepCop](https://github.com/realvizu/NsDepCop) for compile time dependency checks

- Enforce Codemetrics rulesets (cyclocomplexity) for projects (see Zeta)

- further implement CatalogImportJob (Job)

- get minimal api + versioning swagger working

  - see `SharedModule:Map()` for a possible solution
  - needed so the generated `ApiClient` is 100% complete

- migrate some controllers to minimal api's

  - define endpoints in `XXXModule:Map()`
  - start with `BrandController` & `ProductController` as they only trigger commands/queries

- add some UI integrating testing with playwrigth (edu/chapsas/integration-testing-final-7.12)

  - https://playwright.dev/dotnet/docs/intro
  - login + products/brands

- .net 8 migration

- DONE Blazor client appinsights 

  - https://github.com/IvanJosipovic/BlazorApplicationInsights
  - https://github.com/microsoft/ApplicationInsights-dotnet/issues/2143
  - https://github.com/dotnet/aspnetcore/issues/5461

- investigate central package management (nuget)

  - https://nicksnettravels.builttoroam.com/central-package-management/
  - https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management

- investigate devcontainers > https://containers.dev/

  - Dev containers can be run locally or remotely, in a private or public cloud. 
  - intro https://www.youtube.com/playlist?list=PLj6YeMhvp2S5G_X6ZyMc8gfXPMFPg3O31 
  - https://code.visualstudio.com/docs/remote/create-dev-container
  - https://containers.dev/implementors/json_reference/
  - https://www.marcusturewicz.com/blog/develop-aspnetcore-web-apps-faster-with-devcontainer/
  - EXAMPLE dotnet https://dev.to/azure/devcontainers-for-azure-and-net-5942

- add tracing (OpenTelemetry)

  - https://sgryphon.gamertheory.net/2022/07/06/instrumenting-net-with-opentelemetry/
  - https://github.com/sgryphon/dotnet-distributed-tracing-examples/tree/main/src/b-jaeger
  - DONE across modules > https://codeopinion.com/distributed-tracing-to-discover-a-distributed-big-ball-of-mud/
  - DONE entity framework (or trace activity in a repo decorator?)
  - DONE messaging tracing (or trace activity in the MessageBrokerBase?)
    - WAIT pulsar OpenTelemetry support > https://github.com/apache/pulsar-dotpulsar/wiki/Tracing
  - AppInsights dependency tracking (telemetryclient)
    - https://docs.microsoft.com/en-us/azure/azure-monitor/app/custom-operations-tracking#outgoing-dependencies-tracking
    - https://devblogs.microsoft.com/dotnet/observability-asp-net-core-apps/
  - DONE OpenTelemetry Jaeger exporter (incl docker-compose and messaging producer/consumer)
    - https://www.mytechramblings.com/posts/getting-started-with-opentelemetry-and-dotnet-core/
    - jaeger alternative exporter:  https://signoz.io/docs/ + https://signoz.io/docs/instrumentation/dotnet/
  - DONE OpenTelemetry AppInsights exporter
    - DONE > (beta) https://docs.microsoft.com/en-us/azure/communication-services/quickstarts/telemetry-application-insights?pivots=programming-language-csharp
  - Jaeger UI customization
    - add links to seq UI + logs by correlationid > link-patterns https://www.jaegertracing.io/docs/1.21/frontend-ui/#link-patterns
    - add logentries (ActivityStarted >> ActivitySource.AddActivityListener) with seq UI url
      - ^^^currently these logs are *not* visible in the console window (due to different thread)

- DONE proper oauth2 support, instead of just Microsoft ASP.NET core Identity
  - use openiddict for own idp, also support keycloak as id
  
- DONE Document modular monolith > http://www.kamilgrzybek.com/design/modular-monolith-primer/ & øhttps://www.jrebel.com/blog/what-is-a-modular-monolith
  - https://www.youtube.com/watch?v=BOvxJaklcr0
- Document domain model attributes > https://www.kamilgrzybek.com/design/clean-domain-model-attributes

- Domain Events
  - BUG: send domain events after changes commited (SaveChanges) -> GenericRepositoryDomainEventsDecorator
  - Outbox pattern -> GenericRepositoryDomainEventsOutboxDecorator -> http://www.kamilgrzybek.com/design/the-outbox-pattern/

- DocumentStorage abstraction + local/cloud implementations
  - DONE Azure Storage Table storage provider
    - Preferred >> [giometrix/TableStorage.Abstractions.TableEntityConverters: Easily convert POCOs (Plain Old CLR Objects) to Azure Table Storage TableEntities and vice versa (github.com)](https://github.com/giometrix/TableStorage.Abstractions.TableEntityConverters)
    - [Writing Complex Objects to Azure Table Storage – doguarslan (wordpress.com)](https://doguarslan.wordpress.com/2016/02/03/writing-complex-objects-to-azure-table-storage/)
  - DONE Azure Blob storage provider
    - [Get started with Azure Blob Storage](https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blob-dotnet-get-started?tabs=azure-ad)
    - query on part/rowkey > [index tags](https://learn.microsoft.com/en-us/azure/storage/blobs/storage-manage-find-blobs?tabs=azure-portal) > [C# Use blob index tags](https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blob-tags)
    - use native [retry policy](https://learn.microsoft.com/en-us/azure/storage/blobs/storage-retry-policy) 
  - DONE Add some provider decorators like retry/timeout (polly) and chaos
  - DONE move to bitdevkit sln
- DONE [Framework] Write integration tests for the various Application.Storage providers (InMemory/SQL/Azure Storage Blobs and Tables)

- Source Code Only NuGet Packages > https://medium.com/@attilah/source-code-only-nuget-packages-8f34a8fb4738

- Provide application & domain meta nuget packages > [How to create a NuGet metapackage (danielwertheim.se)](https://danielwertheim.se/how-to-create-a-nuget-metapackage/)

- use EF Core 7.0 features in EF Repos like ExecuteUpdate/Delete https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew#executeupdate-and-executedelete-bulk-updates

- Setup [CSP](https://scotthelme.co.uk/content-security-policy-an-introduction/) for Blazor https://damienbod.com/2023/05/22/blazor-and-csp/

- Use Azure Monitor/AppInsights OpenTelemetry https://devblogs.microsoft.com/dotnet/azure-monitor-opentelemetry-distro/ + https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-enable?tabs=aspnetcore
    - local: jaeger/seq/grafana?, cloud: AppInsights (logs, traces, metrics)   

- Document Shop Architecture with C4/structurizr as code (DSL)
  - https://www.youtube.com/watch?v=4HEd1EEQLR0
  - https://dev.to/simonbrown/getting-started-with-structurizr-lite-27d0
  
- [DONE] Use sourcegenerated logging in critical paths (e.g. middleware, repository) > https://learn.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator

- [openapi] bundle, join & generate docs with @redocly/cli
    - resolve redocly join issue https://github.com/Redocly/redocly-cli/issues/1196#issuecomment-1658841444
    - npx @redocly/cli join Presentation.Web.Server\wwwroot\swagger\*.yaml -o .\OpenAPI.yaml
    - npx @redocly/cli join Presentation.Web.Server\wwwroot\swagger\*.yaml -o .\OpenAPI.yaml
    - npx @redocly/cli build-docs .\OpenAPI.yaml -o .\OpenAPI.html
    - OR docker run --rm -v .:/spec redocly/cli:latest join ./*.yaml -o .\OpenAPI.yaml
  - [perf] replace npx with bunx https://bun.sh/docs/cli/bunx
  - 

- provide a mail sender abstraction based on mailkit and with outbox pattern
    - mailkit+mailgrid (azure)
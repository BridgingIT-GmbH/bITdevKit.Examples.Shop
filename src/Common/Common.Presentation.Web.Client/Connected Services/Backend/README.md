### Update the generated API client:
- update API client:
- Install [NSwagStudio](https://github.com/RicoSuter/NSwag/releases/tag/v13.15.10) 
- Start the Presentation.Web project, this will host the backend API and it's swagger documentation
- Open a `cmd` console (in .\src\Presentation.Web.Client\Connected Services\Backend)
- `nswag run /runtime:Net60`
- [ApiClient.cs](.\src\Common\Common.Presentation.Web.Client\Connected Services\Backend\ApiClient.cs) has been updated according to the swagger documentation.
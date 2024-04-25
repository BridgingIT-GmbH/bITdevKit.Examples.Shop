# Azure Bicep infrastructure provisioning

Features:

1. provision Azure resources used by a WebApp using [Azure BICEP](https://github.com/Azure/bicep)
2. use bicep parameter files (json)
3. assign roles to a managed identity (the webapp) to securely interact with a storage account
4. assign policies to the KeyVault for management tasks
5. build and deploy (CI/CD) the WebApp using [Azure Pipelines](https://docs.microsoft.com/en-us/azure/devops/pipelines/?view=azure-devops).

All the Bicep and Pipeline files are located in the `.azure` folder.

## Azure Resources

The following resources are provisioned in Azure:

1. Azure App Service Plan (server)
2. Azure App Service (webapp: presentation.web)
3. Azure App Service (docker: seq)
4. Azure Application Insights + Log Analytics Workspace
5. Azure KeyVault
6. Azure Storage Account (for blob storage)
7. Azure SQL Server
8. Azure SQL Database

## Infrastructure Validation Process

We run a set of tests on the Bicep files to ensure they conform to our required standards.

1. run `build.ps1` to build and validate the Bicep files into the common ARM Templates JSON files
2. run `clean.ps1` to remove all temporary files

## Deploying the Bicep Template

Bicep files are used to describe the Azure resource we require for this project and how they are related to one another.

To deploy the Bicep scipts

1. run `provision.ps1` with all the required parameters

For example:

```ps
az login
az account set --subscription 22463ad3-c6d2-4e7e-911d-c50474250fa0
./provision.ps1 -resourcegroup "rg-dev-bitbaukasten-shop" -location "westeurope" -environment "dev" -bicepFile .\main.bicep -bicepParametersFile .\main.parameters.dev.json -dryrun
```

## Managed Identities and RBAC

In this project we make use of Managed Identities and Role-based Access Control (RBAC) to allow the WebApp to securely interact with other resources (Keyvault, Storage, SqlServer) using the minimim set of operation permissions without us having to maintain a security key or secure connection strings.
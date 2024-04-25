@description('Name of this project')
param projectName string

@description('The environment that this project is being deployed for. (eg. dev, tst, prd)')
param environment string

@description('Datacenter location for your azure resources')
param location string = resourceGroup().location

@description('Resource tags for organizing / cost monitoring')
param tags object

@description('Id of the managed identity')
param identityId string = ''

@description('Name of the KeyVault instance where we want to store secrets')
param keyVaultName string

@description('The full name of the application insights resource')
param appInsightsName string = ''

param appSettings object = {}

@allowed(['api', 'app', 'app,linux', 'app,linux,container', 'functionapp', 'functionapp,linux'])
param kind string = 'app,linux'

param allowedOrigins array = []

param minimumElasticInstanceCount int = -1

param numberOfWorkers int = -1

param clientAffinityEnabled bool = false

param healthCheckPath string = '/health'

@allowed(['B1', 'B2', 'B3', 'D1', 'F1', 'FREE', 'I1', 'I1v2', 'I2', 'I2v2', 'I3', 'I3v2', 'P1V2', 'P1V3', 'P2V2', 'P2V3', 'P3V2', 'P3V3', 'PC2', 'PC3', 'PC4', 'S1', 'S2', 'S3'])
param performanceTier string = 'B3'

@description('All environments use the same app service plan instance')
param sharedAppServicePlan bool = false

var resourceToken = toLower(uniqueString(subscription().id, location, environment, projectName))
var resourceTokenShared = toLower(uniqueString(subscription().id, location, projectName))
var abbrs = loadJsonContent('../abbreviations.json')
var prefix = contains(kind, 'function') ? abbrs.webSitesFunctions : abbrs.webSitesAppService

resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: sharedAppServicePlan ? '${abbrs.webServerFarms}${resourceTokenShared}' : '${environment}-${abbrs.webServerFarms}${resourceToken}'
  location: location
  tags: tags
  sku: {
    name: performanceTier
    capacity: 1
  }
  kind: contains(kind, 'linux') ? 'linux' : 'windows'
  properties: {
    reserved: true
  }
}

resource appService 'Microsoft.Web/sites@2022-09-01' = {
  name: '${environment}-${prefix}${projectName}-${resourceToken}'
  location: location
  tags: tags
  kind: kind
  identity: empty(identityId) ? {
    type: 'SystemAssigned'
  } : {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${identityId}': {}
    }
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    clientAffinityEnabled: clientAffinityEnabled
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|7.0'
      healthCheckPath: healthCheckPath
      alwaysOn: true
      ftpsState: 'Disabled'
      // http20Enabled: true
      minTlsVersion: '1.2'
      numberOfWorkers: numberOfWorkers != -1 ? numberOfWorkers : null
      minimumElasticInstanceCount: minimumElasticInstanceCount != -1 ? minimumElasticInstanceCount : null
      cors: {
        allowedOrigins: union([ 'https://portal.azure.com', 'https://ms.portal.azure.com' ], allowedOrigins)
      }
    }
  }

  resource appSettings 'config' = {
    name: 'appsettings'
    properties: union(
      {
        ASPNETCORE_ENVIRONMENT: environment
      },
      // {
      //   WEBSITE_RUN_FROM_PACKAGE: 1
      // },
      // {
      //   Modules__Shared__Messaging__Pulsar: 'pulsar://localhost:6650'
      // },
      // {
      //   Modules__Catalog__ConnectionStrings__SqlServer: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=ConnectionStrings--SqlServer)'
      // },
      // {
      //   Modules__Identity__ConnectionStrings__SqlServer: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=ConnectionStrings--SqlServer)'
      // },
      // {
      //   Modules__Inventory__ConnectionStrings__LiteDb: 'Filename=data_inventory.db;Connection=shared'
      // },
      // {
      //   Modules__Ordering__ConnectionStrings__SqlServer: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=ConnectionStrings--SqlServer)'
      // },
      // {
      //   Modules__Shopping__ConnectionStrings__SqlServer: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=ConnectionStrings--SqlServer)'
      // },
      !(empty(appInsightsName)) ? { APPLICATIONINSIGHTS_CONNECTION_STRING: appInsights.properties.ConnectionString } : {},
      // !(empty(appInsightsName)) ? { APPLICATIONINSIGHTS_INSTRUMENTATIONKEY: appInsights.properties.InstrumentationKey } : {},
      !(empty(appInsightsName)) ? { APPINSIGHTS_INSTRUMENTATIONKEY: appInsights.properties.InstrumentationKey } : {}, // enables: Application Insights is linked through Instrumentation Key in app settings
      !(empty(keyVaultName)) ? { AzureKeyVault__Name: keyVault.name } : {},
      !(empty(keyVaultName)) ? { AZURE_KEY_VAULT_ENDPOINT: keyVault.properties.vaultUri } : {})
  }
}

module appSettingsUnion 'appservice-settings.bicep' = if (!empty(appSettings)) {
  name: '${appService.name}-appsettings'
  params: {
    appServiceName: appService.name
    currentAppSettings: appService::appSettings.list().properties
    appSettings: appSettings
  }
}

resource siteConfigLogs 'Microsoft.Web/sites/config@2022-09-01' = {
  name: 'logs'
  parent: appService
  properties: {
    applicationLogs: { fileSystem: { level: 'Verbose' } }
    detailedErrorMessages: { enabled: true }
    failedRequestsTracing: { enabled: true }
    httpLogs: { fileSystem: { enabled: true, retentionInDays: 3, retentionInMb: 100 } }
  }
}

resource keyVaultAccessPolicy 'Microsoft.KeyVault/vaults/accessPolicies@2023-07-01' = if (empty(identityId)) {
  name: 'add'
  parent: keyVault
  properties: {
    accessPolicies: [
      {
        tenantId: appService.identity.tenantId
        objectId: appService.identity.principalId
        permissions: {
          keys: [
            'Get'
            'List'
          ]
          secrets: [
            'Get'
            'List'
          ]
          certificates: [
            'Get'
            'List'
          ]
        }
      }
    ]
  }
}

// var storageBlobDataContributorRoleName = 'ba92f5b4-2d11-453d-a403-e96b0029c9fe' // Storage Blob Data Contributor https://docs.microsoft.com/azure/role-based-access-control/built-in-roles

// resource storageBlobContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-02-01' existing = {
//   name: storageBlobContainerName
// }

// resource storageBlobStorageRole 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
//   name: guid(storageBlobDataContributorRoleName, storageBlobContainer.id)
//   scope: storageBlobContainer
//   properties: {
//     principalType: 'ServicePrincipal'
//     roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', storageBlobDataContributorRoleName)
//     principalId: webAppPresentation.identity.principalId
//   }
// }

// var monitoringMetricsPublisherRoleName = '3913510d-42f4-4e42-8a64-420c390055eb' // Monitoring Metrics Publisher https://docs.microsoft.com/azure/role-based-access-control/built-in-roles

// resource monitoringMetricsPublisherRole 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
//   name: guid(monitoringMetricsPublisherRoleName, appInsights.id)
//   scope: appInsights
//   properties: {
//     principalType: 'ServicePrincipal'
//     roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', monitoringMetricsPublisherRoleName)
//     principalId: webAppPresentation.identity.principalId
//   }
// }

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = if (!(empty(appInsightsName))) {
  name: appInsightsName
}

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = if (!(empty(keyVaultName))) {
  name: keyVaultName
}

output appServicePlanName string = appServicePlan.name
output appServicePlanId string = appServicePlan.id
output appServiceName string = appService.name
output appServiceUrl string = 'https://${appService.properties.defaultHostName}'

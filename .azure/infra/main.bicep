@minLength(1)
@description('Name of this project')
param resourceGroup string

@minLength(3)
@maxLength(21)
@description('Name of this project')
param projectName string

@minLength(1)
@description('Datacenter location for your azure resources')
param location string = az.resourceGroup().location // https://azuretracks.com/2021/04/current-azure-region-names-reference/

@allowed(['dev', 'tst', 'prd'])
@description('The environment that this project is being deployed to. (eg. Dev, Test, Staging, Prod)')
param environment string = 'dev'

param version string = '1.0.0'

@description('All environments use the same app service plan instance')
param sharedAppServicePlan bool = false

@description('The desired performance tier for the storage')
@allowed(['Standard_LRS', 'Standard_GRS', 'Standard_RAGRS', 'Standard_ZRS', 'Premium_LRS', 'Premium_ZRS', 'Standard_GZRS', 'Standard_RAGZRS'])
param storagePerformanceTier string = 'Standard_LRS'

@description('The desired performance tier for the SQL Database')
@allowed(['Basic', 'S0', 'S1', 'S2', 'P1', 'P2', 'P3'])
param databasePerformanceTier string = 'Basic'

@description('The desired performance tier for the WebApp')
@allowed(['B1', 'B2', 'B3', 'D1', 'F1', 'FREE', 'I1', 'I1v2', 'I2', 'I2v2', 'I3', 'I3v2', 'P1V2', 'P1V3', 'P2V2', 'P2V3', 'P3V2', 'P3V3', 'PC2', 'PC3', 'PC4', 'S1', 'S2', 'S3'])
param webappPerformanceTier string = 'B2'

@description('Date timestamp of when this deployment was run - defaults to UtcNow()')
param deploymentDate string = utcNow('yyMMddHHmmss')

@description('Resource tags for organizing / cost monitoring')
param tags object = {
  project: projectName
  environment: environment
  resourceToken: toLower(uniqueString(subscription().id, location, environment, projectName))
  version: version
  deploymentDate: deploymentDate
}

@description('Accounts which have keyvault access')
param keyVaultAccessPolicyAccounts object // defined in ./main.parameters.ENV.json

//var abbrs = loadJsonContent('./abbreviations.json')
var resourceGroupName = resourceGroup //sharedResourceGroup ? '${abbrs.resourcesResourceGroups}-${projectName}' : '${abbrs.resourcesResourceGroups}${environment}-${projectName}'

module identity 'modules/identity-system.bicep' = {
  name: '${projectName}-managedIdentity-${deploymentDate}'
  scope: az.resourceGroup(resourceGroupName)
  params: {
    location: location
    environment: environment
    projectName: projectName
    tags: tags
  }
}

module keyVault 'modules/keyvault.bicep' = {
  name: '${projectName}-keyVault-${deploymentDate}'
  scope: az.resourceGroup(resourceGroupName)
  params: {
    location: location
    environment: environment
    projectName: projectName
    tags: tags
    identityId: identity.outputs.identityId
    accessPolicyAccounts: keyVaultAccessPolicyAccounts
  }
}

// ░█▀▀░▀█▀░█▀█░█▀▄░█▀█░█▀▀░█▀▀
// ░▀▀█░░█░░█░█░█▀▄░█▀█░█░█░█▀▀
// ░▀▀▀░░▀░░▀▀▀░▀░▀░▀░▀░▀▀▀░▀▀▀
module storage 'modules/storage.bicep' = {
  name: '${projectName}-storage-${deploymentDate}'
  scope: az.resourceGroup(resourceGroupName)
  params: {
    location: location
    environment: environment
    projectName: projectName
    tags: tags
    identityId: identity.outputs.identityId
    performanceTier: storagePerformanceTier
    keyVaultName: keyVault.outputs.keyVaultName
  }
  dependsOn:[
    keyVault
  ]
}

// ░█▀▀░▄▀▄░█░░░█▀▀░█▀▀░█▀▄░█░█░█▀▀░█▀▄
// ░▀▀█░█\█░█░░░▀▀█░█▀▀░█▀▄░▀▄▀░█▀▀░█▀▄
// ░▀▀▀░░▀\░▀▀▀░▀▀▀░▀▀▀░▀░▀░░▀░░▀▀▀░▀░▀
module database 'modules/sqldatabase.bicep' = {
  name: '${projectName}-database-${deploymentDate}'
  scope: az.resourceGroup(resourceGroupName)
  params: {
    location: location
    environment: environment
    projectName: projectName
    tags: tags
    identityName: identity.outputs.identityName
    identityClientId: identity.outputs.identityClientId
    performanceTier: databasePerformanceTier
    keyVaultName: keyVault.outputs.keyVaultName
  }
  dependsOn:[
    keyVault
  ]
}

// ░█░█░█▀▀░█▀▄░█▀▀░▀█▀░▀█▀░█▀▀
// ░█▄█░█▀▀░█▀▄░▀▀█░░█░░░█░░█▀▀
// ░▀░▀░▀▀▀░▀▀░░▀▀▀░▀▀▀░░▀░░▀▀▀
module appInsights 'modules/appinsights.bicep' = {
  name: '${projectName}-appinsights-${deploymentDate}'
  scope: az.resourceGroup(resourceGroupName)
  params: {
    location: location
    environment: environment
    projectName: projectName
    tags: tags
  }
}

module appInsightsDashboard 'modules/appinsights-dashboard.bicep' = {
  name: '${projectName}-appinsights-dashboard-${deploymentDate}'
  scope: az.resourceGroup(resourceGroupName)
  params: {
    location: location
    environment: environment
    projectName: projectName
    tags: tags
    appInsightsName: appInsights.outputs.appInsightsName
  }
}

module appService 'modules/appservice.bicep' = {
  name: '${projectName}-appservice-${deploymentDate}'
  scope: az.resourceGroup(resourceGroupName)
  params: {
    location: location
    environment: environment
    projectName: projectName
    tags: tags
    identityId: identity.outputs.identityId
    performanceTier: webappPerformanceTier
    appInsightsName: appInsights.outputs.appInsightsName
    keyVaultName: keyVault.outputs.keyVaultName
    sharedAppServicePlan: sharedAppServicePlan
    appSettings: {
          Modules__Shared__Messaging__Pulsar: 'pulsar://localhost:6650'
          Modules__Catalog__ConnectionStrings__SqlServer: '@Microsoft.KeyVault(VaultName=${keyVault.outputs.keyVaultName};SecretName=ConnectionStrings--SqlServer)'
          Modules__Identity__ConnectionStrings__SqlServer: '@Microsoft.KeyVault(VaultName=${keyVault.outputs.keyVaultName};SecretName=ConnectionStrings--SqlServer)'
          Modules__Inventory__ConnectionStrings__LiteDb: 'Filename=data_inventory.db;Connection=shared'
          Modules__Ordering__ConnectionStrings__SqlServer: '@Microsoft.KeyVault(VaultName=${keyVault.outputs.keyVaultName};SecretName=ConnectionStrings--SqlServer)'
          Modules__Shopping__ConnectionStrings__SqlServer: '@Microsoft.KeyVault(VaultName=${keyVault.outputs.keyVaultName};SecretName=ConnectionStrings--SqlServer)'
          Modules__Shopping__ConnectionStrings__StorageAccount: '@Microsoft.KeyVault(VaultName=${keyVault.outputs.keyVaultName};SecretName=ConnectionStrings--StorageAccount)'
    }
  }
  dependsOn:[
    storage
    appInsights
    keyVault
  ]
}

module appServiceSeq 'modules/appservice-container-seq.bicep' = {
  name: '${projectName}-appservice-seq-${deploymentDate}'
  scope: az.resourceGroup(resourceGroupName)
  params: {
    location: location
    environment: environment
    projectName: projectName
    tags: tags
    appServicePlanId: appService.outputs.appServicePlanId
    storageAccountName: storage.outputs.storageAccountName
    storageFileShareName: storage.outputs.storageFileShareName
    // storageBlobContainerName: storage.outputs.storageBlobContainerName
  }
  dependsOn:[
    storage
  ]
}

output resourceGroup string = resourceGroupName
output appServiceName string = appService.outputs.appServiceName
output appServiceUrl string = appService.outputs.appServiceUrl

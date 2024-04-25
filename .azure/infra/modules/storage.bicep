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

@allowed(['Standard_LRS', 'Standard_GRS', 'Standard_RAGRS', 'Standard_ZRS', 'Premium_LRS', 'Premium_ZRS', 'Standard_GZRS', 'Standard_RAGZRS'])
param performanceTier string = 'Standard_LRS'

//var entropy = uniqueString('${subscription().id}${resourceGroup().id}${environment}') // randomness added to the end of storage account name
// var storageAccountName = replace(replace(toLower(take('${projectName}${entropy}', 24)), '-', ''), '_', '')
var abbrs = loadJsonContent('../abbreviations.json')
var resourceToken = toLower(uniqueString(subscription().id, location, environment, projectName))
var storageBlobContainerName = 'container'
var storageFileShareName = 'files'

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
#disable-next-line BCP334
  name: take(replace('${environment}${abbrs.storageStorageAccounts}${resourceToken}', '-', ''), 24)
  location: location
  kind: 'StorageV2'
  tags: tags
  sku: {
    name: performanceTier
  }
  properties: {
    allowSharedKeyAccess: true
    minimumTlsVersion: 'TLS1_2'
    accessTier: 'Hot'
    allowBlobPublicAccess: true
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }
    supportsHttpsTrafficOnly: true
  }
}

resource storageBlobServices 'Microsoft.Storage/storageAccounts/blobServices@2021-04-01' = {
  parent: storageAccount
  name: 'default'
  properties: {
    changeFeed: {
      enabled: false
    }
    restorePolicy: {
      enabled: false
    }
    cors: {
      corsRules: []
    }
    isVersioningEnabled: false
  }
}

resource storageQueueServices 'Microsoft.Storage/storageAccounts/queueServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
  properties: {
    cors: {
      corsRules: []
    }
  }
}

resource storageTableServices 'Microsoft.Storage/storageAccounts/tableServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
  properties: {
    cors: {
      corsRules: []
    }
  }
}

resource storageBlobContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-02-01' = {
#disable-next-line use-parent-property
  name: '${storageAccount.name}/default/${storageBlobContainerName}'
  properties: {
    publicAccess: 'Blob'
  }
}

resource storageFileShare 'Microsoft.Storage/storageAccounts/fileServices/shares@2021-06-01' = {
#disable-next-line use-parent-property
  name: '${storageAccount.name}/default/${storageFileShareName}'
  properties: {
    accessTier: 'TransactionOptimized'
  }
}

resource keyvault 'Microsoft.KeyVault/vaults@2023-07-01' existing = if (!(empty(keyVaultName))) {
  name: keyVaultName

  resource storageAccountConnectionStringSecret 'secrets' = {
    name: 'ConnectionStrings--StorageAccount'
    tags: tags
    properties: {
      value: storageConnectionString
      attributes: {
        enabled: true
      }
    }
  }
}

resource storageAccountContributorRoleDefinition 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  scope: subscription()
  name: '17d1049b-9a84-46fb-8f53-869881c3d3ab' // Storage Account Contributor
}

resource tableRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = if (!(empty(identityId))) {
  name: guid(resourceToken, storageAccountContributorRoleDefinition.id)
  scope: storageAccount
  properties: {
    roleDefinitionId: storageAccountContributorRoleDefinition.id
    principalId: identityId
  }
}

resource storageBlobDataContributorRoleDefinition 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  scope: subscription()
  name: 'ba92f5b4-2d11-453d-a403-e96b0029c9fe' // Storage Blob Data Contributor
}

resource blobRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = if (!(empty(identityId))) {
  name: guid(resourceToken, storageBlobDataContributorRoleDefinition.id)
  scope: storageAccount
  properties: {
    roleDefinitionId: storageBlobDataContributorRoleDefinition.id
    principalId: identityId
  }
}

resource storageQueueDataContributorRoleDefinition 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  scope: subscription()
  name: '974c5e8b-45b9-4653-ba55-5f855dd0fb88' // Storage Queue Data Contributor
}

resource QueueRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = if (!(empty(identityId))) {
  name: guid(resourceToken, storageQueueDataContributorRoleDefinition.id)
  scope: storageAccount
  properties: {
    roleDefinitionId: storageQueueDataContributorRoleDefinition.id
    principalId: identityId
  }
}

resource storageTableDataContributorRoleDefinition 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  scope: subscription()
  name: '0a9a7e1f-b9d0-4cc4-a60d-0319b160aaa3' // Storage Table Data Contributor
}

resource TableRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = if (!(empty(identityId))) {
  name: guid(resourceToken, storageTableDataContributorRoleDefinition.id)
  scope: storageAccount
  properties: {
    roleDefinitionId: storageTableDataContributorRoleDefinition.id
    principalId: identityId
  }
}

var storageConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=core.windows.net;AccountKey=${storageAccount.listKeys().keys[0].value}'

output storageAccountName string = storageAccount.name
output storageBlobContainerName string = storageBlobContainerName
output storageFileShareName string = storageFileShareName
output storageConnectionStringKey string = storageConnectionString

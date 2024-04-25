@description('Name of this project')
param projectName string

@description('The environment that this project is being deployed for. (eg. dev, tst, prd)')
param environment string

@description('Datacenter location for your azure resources')
param location string = resourceGroup().location

@description('Resource tags for organizing / cost monitoring')
param tags object

param appServicePlanId string

@description('The full name of the storage account')
param storageAccountName string

@description('The full name of the storage file share')
param storageFileShareName string

var abbrs = loadJsonContent('../abbreviations.json')
var resourceToken = toLower(uniqueString(subscription().id, location, environment, projectName))

resource appServiceSeq 'Microsoft.Web/sites@2022-09-01' = {
  name: '${environment}-${abbrs.containerSeq}${projectName}-${resourceToken}'
  location: location
  tags: tags
  kind: 'app,linux,container'
  properties: {
    serverFarmId: appServicePlanId
    siteConfig: {
      linuxFxVersion: 'DOCKER|datalust/seq:latest'
      alwaysOn: true
      appSettings: [
        {
          name: 'ACCEPT_EULA'
          value: 'Y'
        }
        {
          name: 'WEBSITES_PORT' // https://learn.microsoft.com/en-us/azure/app-service/configure-custom-container?pivots=container-linux#configure-port-number
          value: '80:5341'
        }
      ]
      azureStorageAccounts: { // storage mount https://docs.microsoft.com/en-us/rest/api/appservice/web-apps/update-azure-storage-accounts
        data: {
          type: 'AzureFiles'
          accountName: storageAccountName
          shareName: storageFileShareName
          mountPath: '/data'
          accessKey: storageAccount.listKeys().keys[0].value
        }
      }
    }
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

output appServiceSeqName string = appServiceSeq.name
output appServiceSeqUrl string = 'https://${appServiceSeq.properties.defaultHostName}'

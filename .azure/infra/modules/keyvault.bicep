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

@description('Accounts which have keyvault access')
param accessPolicyAccounts object

// @secure()
// param secretApiKey string = ''

var abbrs = loadJsonContent('../abbreviations.json')
var resourceToken = toLower(uniqueString(subscription().id, location, environment, projectName))
// var accessPolicyAccounts = {
//   'principal name': 'object id (upn)'
// }

var accessPolicies = [for objectId in items(accessPolicyAccounts): {
  tenantId: subscription().tenantId
  objectId: objectId.value
  permissions: {
      keys: [
        'Get'
        'List'
        'Update'
        'Create'
        'Import'
        'Delete'
        'Recover'
        'Backup'
        'Restore'
      ]
      secrets: [
        'Get'
        'List'
        'Set'
        'Delete'
        'Recover'
        'Backup'
        'Restore'
      ]
      certificates: [
        'Get'
        'List'
        'Update'
        'Create'
        'Import'
        'Delete'
        'Recover'
        'Backup'
        'Restore'
      ]
  }
}]

var accessPoliciesIdentities = [
  {
    tenantId: subscription().tenantId
    objectId: identityId
    permissions: {
        keys: [
          'Get'
          'List'
          'Update'
          'Create'
          'Import'
          'Delete'
          'Recover'
          'Backup'
          'Restore'
        ]
        secrets: [
          'Get'
          'List'
          'Set'
          'Delete'
          'Recover'
          'Backup'
          'Restore'
        ]
        certificates: [
          'Get'
          'List'
          'Update'
          'Create'
          'Import'
          'Delete'
          'Recover'
          'Backup'
          'Restore'
        ]
    }
  }
]

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: take('${environment}-${abbrs.keyVaultVaults}${resourceToken}', 24)
  location: location
  tags: tags
  properties: {
    tenantId: subscription().tenantId
    enabledForTemplateDeployment: true
    createMode: 'default'
    accessPolicies: empty(identityId) ? accessPolicies : concat(accessPolicies, accessPoliciesIdentities)
    sku: {
      name: 'standard'
      family: 'A'
    }
  }
}

output keyVaultName string = keyVault.name
output keyVaultUri string = keyVault.properties.vaultUri

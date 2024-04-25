@description('Name of this project')
param projectName string

@description('The environment that this project is being deployed for. (eg. dev, tst, prd)')
param environment string

@description('Datacenter location for your azure resources')
param location string = resourceGroup().location

@description('Resource tags for organizing / cost monitoring')
param tags object

var abbrs = loadJsonContent('../abbreviations.json')
var resourceToken = toLower(uniqueString(subscription().id, location, environment, projectName))

// User assigned managed identity
resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: '${environment}-${abbrs.managedIdentityUserAssignedIdentities}${resourceToken}'
  location: location
  tags: tags
}

// Grants full access to manage all resources
resource contributorRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceToken)
  properties: {
    principalType: 'ServicePrincipal'
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b24988ac-6180-42a0-ab88-20f7382dd24c')
    principalId: identity.properties.principalId
  }
}

output identityId string = identity.id
output identityPrincipalId string = identity.properties.principalId
output identityName string = identity.name
output identityClientId string = identity.properties.clientId

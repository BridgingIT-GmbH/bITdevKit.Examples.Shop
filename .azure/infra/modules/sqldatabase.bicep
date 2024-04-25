@description('Name of this project')
param projectName string

@description('The environment that this project is being deployed for. (eg. dev, tst, prd)')
param environment string

@description('Datacenter location for your azure resources')
param location string = resourceGroup().location

@description('Resource tags for organizing / cost monitoring')
param tags object

@description('Name of the managed identity')
param identityName string = ''

@description('ClientId of the managed identity')
param identityClientId string = ''

@allowed(['Basic', 'S0', 'S1', 'S2', 'P1', 'P2', 'P3'])
param performanceTier string = 'Basic'

@description('Name of the KeyVault instance where we want to store secrets')
param keyVaultName string

@description('Date timestamp of when this deployment was run - defaults to UtcNow()')
param timestamp string = utcNow('yyMMddHHmmss')

var abbrs = loadJsonContent('../abbreviations.json')
var resourceToken = toLower(uniqueString(subscription().id, location, environment, projectName))
var sqlAdminUsername = 'sqladmin'
var entropy = uniqueString(timestamp, resourceToken)
var sqlAdminPassword = '#A${take(entropy,12)}!'
#disable-next-line no-unused-vars
var sqlUsername = 'sqluser'
var sqlUserPassword = '#U${take(entropy,12)}!'

resource sqlServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: '${environment}-${abbrs.sqlServers}${resourceToken}'
  location: location
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    administratorLogin: sqlAdminUsername
    administratorLoginPassword: sqlAdminPassword
    administrators: empty(identityClientId) ? null : {
      azureADOnlyAuthentication: false
      login: identityName
      administratorType: 'ActiveDirectory'
      principalType: 'Application'
      sid: identityClientId
      tenantId: tenant().tenantId
    }
  }

  resource database 'databases' = {
    name: '${environment}-${abbrs.sqlServersDatabases}${resourceToken}'
    location: location
    tags: tags
    sku: {
      name: performanceTier
    }
    properties: {
      collation: 'SQL_Latin1_General_CP1_CI_AS'
      catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    }
  }
}

// resource sqlDeploymentScript 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//   name: '${environment}-${abbrs.sqlServersDatabasesScript}-${resourceToken}'
//   location: location
//   tags: tags
//   kind: 'AzureCLI'
//   //identity:
//   properties: {
//     azCliVersion: '2.37.0'
//     retentionInterval: 'PT1H' // Retain the script resource for 1 hour after it ends running
//     timeout: 'PT5M' // Five minutes
//     cleanupPreference: 'OnSuccess'
//     environmentVariables: [
//       {
//         name: 'DBSERVER'
//         value: sqlServer.properties.fullyQualifiedDomainName
//       }
//       {
//         name: 'DBNAME'
//         value: sqlServer::database.name
//       }
//       {
//         name: 'APPUSERNAME'
//         value: sqlUsername
//       }
//       {
//         name: 'APPUSERPASSWORD'
//         secureValue: sqlUserPassword
//       }
//       {
//         name: 'SQLADMIN'
//         value: sqlAdminUsername
//       }
//       {
//         name: 'SQLCMDPASSWORD'
//         secureValue: sqlAdminPassword
//       }
//     ]

//     scriptContent: '''
// wget https://github.com/microsoft/go-sqlcmd/releases/download/v0.8.1/sqlcmd-v0.8.1-linux-x64.tar.bz2
// tar x -f sqlcmd-v0.8.1-linux-x64.tar.bz2 -C .

// cat <<SCRIPT_END > ./initDb.sql
// drop user ${APPUSERNAME}
// go
// create user ${APPUSERNAME} with password = '${APPUSERPASSWORD}'
// go
// alter role db_owner add member ${APPUSERNAME}
// go
// SCRIPT_END

// ./sqlcmd -S ${DBSERVER} -d ${DBNAME} -U ${SQLADMIN} -i ./initDb.sql
//     '''
//   }
// }

resource sqlServerFirewallAzureServices 'Microsoft.Sql/servers/firewallRules@2022-05-01-preview' = {
  parent: sqlServer
  name: 'AllowAllWindowsAzureIps'
  properties: {
    endIpAddress: '0.0.0.0'
    startIpAddress: '0.0.0.0'
  }
}

resource sqlServerFirewallClient 'Microsoft.Sql/servers/firewallRules@2022-05-01-preview' = {
  parent: sqlServer
  name: 'ClientIps'
  properties: {
    startIpAddress: '178.0.0.0'
    endIpAddress: '178.255.255.255'
  }
}

resource keyvault 'Microsoft.KeyVault/vaults@2023-07-01' existing = if (!(empty(keyVaultName))) {
  name: keyVaultName

  resource sqlAdminPasswordSecret 'secrets' = {
    name: 'sqlAdminPassword'
    tags: tags
    properties: {
      value: sqlAdminPassword
      attributes: {
        enabled: true
      }
    }
  }

  resource sqlUserPasswordSecret 'secrets' = {
    name: 'sqlUserPassword'
    tags: tags
    properties: {
      value: sqlUserPassword
      attributes: {
        enabled: true
      }
    }
  }

  resource storageAccountConnectionStringSecret 'secrets' = {
    name: 'ConnectionStrings--SqlServer'
    tags: tags
    properties: {
      value: sqlConnectionString
      attributes: {
        enabled: true
      }
    }
  }
}

var sqlConnectionString = empty(identityClientId)
  ? 'Server=${sqlServer.properties.fullyQualifiedDomainName};Database=${sqlServer::database.name};User=${sqlAdminUsername};Password=${sqlAdminPassword};'
  : 'Server=${sqlServer.properties.fullyQualifiedDomainName};Database=${sqlServer::database.name};User Id=${identityClientId};Authentication=Active Directory Managed Identity;Encrypt=True;'

output sqlConnectionStringKey string = sqlConnectionString

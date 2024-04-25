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

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' = { // https://github.com/Azure/azure-quickstart-templates/tree/master/quickstarts/microsoft.web/web-app-loganalytics
  name: '${environment}-${abbrs.operationalInsightsWorkspaces}${resourceToken}'
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 90
    workspaceCapping: {
      dailyQuotaGb: 1
    }
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = { // https://markheath.net/post/azure-functions-bicep
  name: '${environment}-${abbrs.insightsComponents}${resourceToken}'
  location: location
  kind: 'web'
  tags: tags
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalytics.id
  }
  // tags: {
  //   displayName: 'AppInsights'
  //   project: toLower(projectName)
  //   environment: toLower(environment)
  //   'hidden-link:/subscriptions/${subscription().id}/resourceGroups/${resourceGroup().name}/providers/Microsoft.Web/sites/${appServiceName}': 'Resource'
  // }
}

output logAnalyticsWorkspaceId string = logAnalytics.id
output logAnalyticsWorkspaceName string = logAnalytics.name
output appInsightsConnectionString string = appInsights.properties.ConnectionString
output appInsightsInstrumentationKey string = appInsights.properties.InstrumentationKey
output appInsightsName string = appInsights.name

param appServiceName string
param appSettings object
param currentAppSettings object

resource siteConfigUnion 'Microsoft.Web/sites/config@2022-09-01' = {
  name: '${appServiceName}/appsettings'
  properties: union(currentAppSettings, appSettings)
}

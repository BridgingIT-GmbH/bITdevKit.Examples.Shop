@description('Name of this project')
#disable-next-line no-unused-params
param projectName string

@description('The environment that this project is being deployed for. (eg. dev, tst, prd)')
#disable-next-line no-unused-params
param environment string

@description('Datacenter location for your azure resources')
#disable-next-line no-unused-params
param location string = resourceGroup().location

#disable-next-line no-unused-params
param tags object

output identityId string = ''
output identityName string = ''
output identityClientId string = ''

[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$resourceGroup,

    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [String]$keyvaultName

    # # Get UPN from Azure Portal -> Azure Active Directory -> Users -> YOU -> Look for 'User Principal Name' field
    # [Parameter(Mandatory = $true)]
    # [ValidateNotNullOrEmpty()]
    # [string]$upn
)

$user = az ad user list --filter "startswith(mailNickname, '$env:USERNAME')" --query "[].{principal:userPrincipalName,upn:objectId}" | ConvertFrom-Json

az keyvault set-policy -g $resourceGroup -n $keyvaultName --upn $user.upn `
    --certificate-permissions backup create delete deleteissuers get getissuers import list listissuers managecontacts manageissuers purge recover restore setissuers update `
    --key-permissions backup create decrypt delete encrypt get import list purge recover restore sign unwrapKey update verify wrapKey `
    --secret-permissions backup delete get list purge recover restore set `
    --storage-permissions backup delete deletesas get getsas list listsas purge recover regeneratekey restore set setsas update
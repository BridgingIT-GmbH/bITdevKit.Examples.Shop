Get-ChildItem -Path '*.bicep' -Recurse | ForEach-Object {
    az bicep build --file $_.FullName
    Write-Host "$($_.FullName) -> $($_.FullName.Replace(".bicep", ".json"))"
}
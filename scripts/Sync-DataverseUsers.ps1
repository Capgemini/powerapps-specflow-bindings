param (
    [Parameter(Mandatory)]
    [String]
    $EnvironmentName,
    [Parameter(Mandatory)]
    [String[]]
    $ObjectIds,
    [Parameter(Mandatory)]
    [String]
    $TenantId,
    [Parameter(Mandatory)]
    [String]
    $ClientId,
    [Parameter(Mandatory)]
    [String]
    $ClientSecret
)

Install-Module -Name Microsoft.PowerApps.Administration.PowerShell -Force -Scope CurrentUser -AllowClobber

Write-Host "Authenticating as $ClientId."
Add-PowerAppsAccount -TenantID $TenantId -ApplicationId $ClientId -ClientSecret $ClientSecret

$ObjectIds | ForEach-Object {
    Write-Host "Syncing $_."
    Add-AdminPowerAppsSyncUser -EnvironmentName $EnvironmentName -PrincipalObjectId $_ | Out-Null
}
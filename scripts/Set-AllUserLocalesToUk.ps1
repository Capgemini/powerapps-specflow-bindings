Install-Module -Name Microsoft.Xrm.Tooling.CrmConnector.PowerShell -Force -Scope CurrentUser -AllowClobber

Write-Host "Updating user settings to English (United Kingdom)"

# $connectionString = "AuthType=OAuth; Username=$env:username; Password=$env:password; Url=$env:environmentUrl; AppId=51f81489-12ee-4a9e-aaae-a2591f45987d; RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97; LoginPrompt=Auto";
$connectionString = "AuthType=ClientSecret; ClientId=$env:clientId; ClientSecret=`"$env:clientSecret`"; Url=$env:environmentUrl";
Write-Host $connectionString

$conn = Get-CrmConnection -ConnectionString $connectionString;

$condition = [Microsoft.Xrm.Sdk.Query.ConditionExpression]::new();  
$condition.AttributeName = "localeid";  
$condition.Operator = [Microsoft.Xrm.Sdk.Query.ConditionOperator]::NotEqual;  
$condition.Values.Add(2057);              
  
$filter = [Microsoft.Xrm.Sdk.Query.FilterExpression]::new();  
$filter.Conditions.Add($condition);  
  
$query = [Microsoft.Xrm.Sdk.Query.QueryExpression]::new("usersettings");  
$query.ColumnSet.AddColumns("localeid");  
$query.Criteria.AddFilter($filter);  

$executeMultipleSettings = [Microsoft.Xrm.Sdk.ExecuteMultipleSettings]::new()
$executeMultipleSettings.ContinueOnError = $true
$executeMultipleSettings.ReturnResponses = $false

$executeMultipleRequests = [Microsoft.Xrm.Sdk.OrganizationRequestCollection]::new()

$usersettings = $conn.RetrieveMultiple($query).Entities
$usersettings | ForEach-Object {
    $_.Attributes['localeid'] = 2057
    $updateRequest = [Microsoft.Xrm.Sdk.Messages.UpdateRequest]::new()
    $updateRequest.Target = $_
    $executeMultipleRequests.Add($updateRequest)
}

$executeMultipleRequest = [Microsoft.Xrm.Sdk.Messages.ExecuteMultipleRequest]::new()
$executeMultipleRequest.Settings = $executeMultipleSettings
$executeMultipleRequest.Requests = $executeMultipleRequests
$conn.Execute($executeMultipleRequest)
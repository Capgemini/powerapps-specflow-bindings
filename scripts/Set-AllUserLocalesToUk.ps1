Install-Module -Name Microsoft.Xrm.Tooling.CrmConnector.PowerShell -Force -Scope CurrentUser -AllowClobber

Write-Host "Updating user settings to English (United Kingdom)"
Write-Host "Url=$env:POWERAPPS_SPECFLOW_BINDINGS_TEST_URL; ClientId=$env:POWERAPPS_SPECFLOW_BINDINGS_TEST_CLIENTID; ClientSecret=$env:POWERAPPS_SPECFLOW_BINDINGS_TEST_CLIENTSECRET; AuthType=ClientSecret"

$conn = Get-CrmConnection -ConnectionString "Url=$env:POWERAPPS_SPECFLOW_BINDINGS_TEST_URL; ClientId=$env:POWERAPPS_SPECFLOW_BINDINGS_TEST_CLIENTID; ClientSecret=$env:POWERAPPS_SPECFLOW_BINDINGS_TEST_CLIENTSECRET; AuthType=ClientSecret"

$condition = [Microsoft.Xrm.Sdk.Query.ConditionExpression]::new();  
$condition.AttributeName = "localeid";  
$condition.Operator = ConditionOperator.NotEqual;  
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
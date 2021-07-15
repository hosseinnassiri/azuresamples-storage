# Using Azure Storage Account to store files as blobs
This is to explain how to leverage Azure Storage Account blobs to store files, using Azure AD applications.
It's a simple ASP .Net Core API application that can upload / download and list the files in a blob storage container in Azure Storage Account which can be deployed anywhere on-premise or cloud.
For authentication and authorization, we use Azure AD Application registration and Client Credential flow of oAuth 2.0 (using Client Id & Client Secret).

## Prepare the infrastructure
We use PowerShell 7.0 and az module.

### Create the storage account
Let's connect to azure first using az PowerShell module:
``` powershell
Import-Module az
Connect-AzAccount
```
We create the resource group:
``` powershell
$resourceGroup = "rg-cace-dev-stgtest-01"
$location = "canadacentral"
New-AzResourceGroup -Name $resourceGroup -Location $location
```

Then we add the storage account:
``` powershell
$storageName = "hosseinteststgd01"
$storageAccount = New-AzStorageAccount -ResourceGroupName $resourceGroup `
  -Name $storageName `
  -SkuName Standard_LRS `
  -Location $location
```

Create a container in our storage account:
``` powershell
$storageAccount = Get-AzStorageAccount -Name $storageName -ResourceGroupName $resourceGroup
New-AzStorageContainer -Name "my-container-01" -Permission Off -Context $storageAccount.Context
Get-AzStorageContainer -Name *contain* -Context $storageAccount.Context
```

### Configure Azure AD application
Create a new Azure AD application:
``` powershell
$adApplication = New-AzADApplication -DisplayName "my-storage-app-01" `
    -IdentifierUris "https://localhost:5001"
```

Adding a new service principal for the Azure AD application which has a Contributor role scoped only to our storage account:
``` powershell
$subscriptionId = (Get-AzContext).Subscription.Id
$servicePrincipal = New-AzADServicePrincipal `
    -ApplicationId $adApplication.ApplicationId `
    -Scope "/subscriptions/$subscriptionId/resourceGroups/$resourceGroup/providers/Microsoft.Storage/storageAccounts/$storageName" `
    -Role Contributor
```

Getting the azure role definition:
``` powershell
Get-AzRoleDefinition | Where-Object {$_.Name -like "Storage*"} | FT Name, IsCustom, Id
$role = Get-AzRoleDefinition "Storage Blob Data Contributor"
```

Assign the **Storage Blob Data Contributor** role to service principal to our storage account:
``` powershell
New-AzRoleAssignment -ApplicationId $servicePrincipal.ApplicationId `
    -RoleDefinitionName  $role.Name `
    -ResourceName $storageName `
    -ResourceType "Microsoft.Storage/storageAccounts" `
    -ResourceGroupName $resourceGroup
```

Create Client Id & Client Secret for the Azure AD Applicatio.

## Sample asp.net core application
Add package references for the following packages:
* Azure.Storage.Blobs
* Azure.Identity

Update your appsettings.json file by adding these key/values :
``` json
"ApplicationCredential": {
    "TenantId": "your tenant id",
    "ClientId": "client id (azure ad application id)",
    "ClientSecret": "client secret",
    "StorageAccountUrl": "url to your storage account",
    "RootContainer": "you container name"
},
```
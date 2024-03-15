# Garbage Collector on Azure App Service Proof of concept

The goal of this POC is to investigate different Garbage Collection settings on a web api and see how the app acts under load running on azure app services.

The solution contains 3 main parts

- ./src/Poc.Garbaga.ServerApi: The dotnet Web Api
- ./src/Poc.Garbaga.Load: small console app for generating some load to the web api
- ./scripts/**: powershell scripts to deploy the needed resources to azure

> **WARNING**
> Don't forget to cleanup your resources after usage.

## Preparing the environment

### Requirements

- [Azure Cli](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli)
- [Dotnet 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

### Login i to Azure

Login with azure cli and select the correct subscription.

```powershell
az login
# replace with the correct subscription id
az account set --subscription "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX"
```

### Set environment variables

Make sure to have environment variables containing the correct values for a resource group, app service plan, and app service prefix. You should change the names in the [script](./scripts/setVariables.ps1) to unique names used for personal use.

Then run the script to set the variables

```powershell
./scripts/setVariables.ps1
```

Also make sure to set the  ```appPrefix``` variable in the [program.cs](./src/Poc.Garbage.Load/Program.cs) to the same value as the ```$ENV:poc_gc_app_prefix``` environment variable in the [setVariables.ps1](./scripts/setVariables.ps1) script.

```csharp
var appPrefix = "app-poc-gc";
```

### Create Azure resources

Before deploying the code, you need to create the resources on Azure. This step is only needed once and is launched by running the [initialize.ps1](./scripts/initialize.ps1) script.

```powershell
./scripts/initialize.ps1
```

## Prepare application

You can tweak GC settings in the [Poc.Garbage.ServerApi.csproj](./src/Poc.Garbage.ServerApi/Poc.Garbage.ServerApi.csproj) file.

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

    <PropertyGroup>
        <TargetFramework>net8.0</TargetFramework>
        <Nullable>enable</Nullable>
        <ImplicitUsings>enable</ImplicitUsings>
        <InvariantGlobalization>true</InvariantGlobalization>
        <ServerGarbageCollection>false</ServerGarbageCollection>
        <!--<ServerGarbageCollection>true</ServerGarbageCollection>-->
        <!--<GarbageCollectionAdaptationMode>1</GarbageCollectionAdaptationMode>-->
    </PropertyGroup>
    
    <!-- <ItemGroup>
        <RuntimeHostConfigurationOption Include="DOTNET_GCHighMemPercent" Value="75" />
    </ItemGroup> -->
```

## Deploy application

You can deploy the applications to Azure by running the [deployToAzure.ps1](./scripts/deployToAzure.ps1) script.

```powershell
./scripts/deployToAzure.ps1
```

## Run Load Test

### Tweak the load

In the [Program.cs](./src/Poc.Garbage.Load/Program.cs) file, you can tweak fine tune the load you want to send to the applications.

```csharp
// The interval represents a time when [rate] amount of requests will be send to ech app
var interval = TimeSpan.FromSeconds(1);
// The number of requests that will be send to the app's per interval
var rate = 10;
// The total amount of time the app's will be under load
var during = TimeSpan.FromMinutes(10);
// The string size that will be created in the api's
var stringSizeInKB = 200;
// The number of strings that will be created in the api
var amountOfStrings = 200;
```

### Run the load test

```powershel
dotnet run --project ./src/Poc.Garbage.Load
```

## Links

- [High Memory Percent](https://learn.microsoft.com/en-us/dotnet/core/runtime-config/garbage-collector#high-memory-percent)
- [Workstation vs Server Garbage Collection](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/workstation-server-gc)
- [DATA](https://maoni0.medium.com/dynamically-adapting-to-application-sizes-2d72fcb6f1ea)

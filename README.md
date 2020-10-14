# DickinsonBros.Telemetry

<a href="https://dev.azure.com/marksamdickinson/dickinsonbros/_build/latest?definitionId=53&amp;branchName=master"> <img alt="Azure DevOps builds (branch)" src="https://img.shields.io/azure-devops/build/marksamdickinson/DickinsonBros/53/master"> </a> <a href="https://dev.azure.com/marksamdickinson/dickinsonbros/_build/latest?definitionId=53&amp;branchName=master"> <img alt="Azure DevOps coverage (branch)" src="https://img.shields.io/azure-devops/coverage/marksamdickinson/dickinsonbros/53/master"> </a><a href="https://dev.azure.com/marksamdickinson/DickinsonBros/_release?_a=releases&view=mine&definitionId=25"> <img alt="Azure DevOps releases" src="https://img.shields.io/azure-devops/release/marksamdickinson/b5a46403-83bb-4d18-987f-81b0483ef43e/25/26"> </a><a href="https://www.nuget.org/packages/DickinsonBros.Telemetry/"><img src="https://img.shields.io/nuget/v/DickinsonBros.Telemetry"></a>

A Telemetry service

Features
* Captures Telemetry (Name, ElapsedMilliseconds, TelemetryType, TelemetryState, DateTime)
* Sends Telemetry to SQL Tables every 30 seconds in bulk inserts.
* Flush Method OR Application Lifetime (ApplicationStopping) - To clear all existing logs

<h2>Example Usage</h2>

```C#
    Console.WriteLine("Insert API Telemetry (50 Times)");
    for (int i = 0; i < 50; i++)
    {
        telemetryService.Insert(GenerateTelemetry());
    }

    Console.WriteLine("Flush Telemetry");
    await telemetryService.FlushAsync().ConfigureAwait(false);
```

    Insert API Telemetry (50 Times)
    Flush Telemetry

![Alt text](https://raw.githubusercontent.com/msdickinson/DickinsonBros.Telemetry/develop/TelemetrySQL.PNG)

Example Runner Included in folder "DickinsonBros.Telemetry.Runner"

<h2>Setup</h2>

<h3>Add nuget references</h3>

    https://www.nuget.org/packages/DickinsonBros.Telemetry
    https://www.nuget.org/packages/DickinsonBros.Telemetry.Abstractions
    
    https://www.nuget.org/packages/DickinsonBros.Logger
    https://www.nuget.org/packages/DickinsonBros.Logger.Abstractions
    
    https://www.nuget.org/packages/DickinsonBros.Redactor
    https://www.nuget.org/packages/DickinsonBros.Redactor.Abstractions

<h3>Create instance with dependency injection</h3>

<h4>Add appsettings.json File With Contents</h4>

Note: Runner Shows this with added steps to enypct Connection String

 ```json  
{
  "TelemetryServiceOptions": {
    "ConnectionString": "..."
  }
}
 ```    
<h4>Code</h4>

```c#

//ApplicationLifetime
using var applicationLifetime = new ApplicationLifetime();

//ServiceCollection
var serviceCollection = new ServiceCollection();

//Configure Options
var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false)

var configuration = builder.Build();
serviceCollection.AddOptions();

services.AddSingleton<IApplicationLifetime>(applicationLifetime);

//Add Logging Service
services.AddLoggingService();

//Add Redactor
services.AddRedactorService();
services.Configure<RedactorServiceOptions>(_configuration.GetSection(nameof(RedactorServiceOptions)));

//Add Telemetry
services.AddTelemetryService();
services.Configure<TelemetryServiceOptions>(_configuration.GetSection(nameof(TelemetryServiceOptions)));

//Build Service Provider 
using (var provider = services.BuildServiceProvider())
{
  var telemetryService = provider.GetRequiredService<ITelemetryService>();
}
```

<h3>Setup Database</h3>

<h4>SQL Database Project</h4>

In DickinsonBros.Telemetry.Database you can publish the tables required or generate a script.

* Open Sln in base of repo with visaul studio 
* Right Click DickinsonBros.Telemetry.Database In Visaul studio and click Publish
* Select Taget database Conneciton
* Click Generate Script or Publish (If you click generate script you will need to run it in your database)

Note: The ConnectionString in TelemetryServiceOptions json above should point to where your database is located.


# DickinsonBros.Telemetry

<a href="https://www.nuget.org/packages/DickinsonBros.Telemetry/">
    <img src="https://img.shields.io/nuget/v/DickinsonBros.Telemetry">
</a>

A Telemetry service

Features
* Captures Telemetry (SQL, Middleware, DurableRest, API, Email, Queue)
* Sends Telemetry to SQL Tables every 30 seconds in bulk inserts.
* Flush Method OR Application Lifetime (ApplicationStopping - To clear all existing logs

<a href="https://dev.azure.com/marksamdickinson/DickinsonBros/_build?definitionScope=%5CDickinsonBros.Telemetry">Builds</a>

<h2>Example Usage</h2>

```C#
  Console.WriteLine("Insert API Telemetry (50 Times)");
  for (int i = 0; i < 50; i++)
  {
      telemetryService.InsertAPI(GenerateAPITelemetry());
  }

  Console.WriteLine("Insert DurableRest Telemetry (50 Times)");
  for (int i = 0; i < 50; i++)
  {
      telemetryService.InsertDurableRest(GenerateDurableRestTelemetry());
  }

  Console.WriteLine("Insert Email Telemetry (50 Times)");
  for (int i = 0; i < 50; i++)
  {
      telemetryService.InsertEmail(GenerateEmailTelemetry());
  }

  Console.WriteLine("Insert Queue Telemetry (50 Times)");
  for (int i = 0; i < 50; i++)
  {
      telemetryService.InsertQueue(GenerateQueueTelemetry());
  }

  Console.WriteLine("Insert SQL Telemetry (50 Times)");
  for (int i = 0; i < 50; i++)
  {
      telemetryService.InsertSQL(GenerateSQLTelemetry());
  }

  Console.WriteLine("Flush Telemetry");


  await telemetryService.Flush().ConfigureAwait(false);
```
  Insert API Telemetry (50 Times)
  Insert DurableRest Telemetry (50 Times)
  Insert Email Telemetry (50 Times)
  Insert Queue Telemetry (50 Times)
  Insert SQL Telemetry (50 Times)
  Flush Telemetry

Example Runner Included in folder "DickinsonBros.Encryption.Telemetry"

<h2>Setup</h2>

<h3>Add Nuget References</h3>

    https://www.nuget.org/packages/DickinsonBros.Telemetry
    https://www.nuget.org/packages/DickinsonBros.Telemetry.Abstractions


<h3>Create Instance</h3>

```c#

//Options
var telemetryServiceOptions = new TelemetryServiceOptions
{
    Source = "DickinsonBros.Telemtry.Runner",
    RecordDurableRest = true,
    RecordSQL = true,
    RecordQueue = true,
    RecordAPI = true,
    RecordEmail = true,
    ConnectionString = "..."
};
var options = Options.Create(telemetryServiceOptions);

//IApplicationLifetime
var applicationLifetime = new ApplicationLifetime();

//ILogger<T>
var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddConsole();
});

//IRedactorService
var redactorServiceOptions = new RedactorServiceOptions
{
    PropertiesToRedact = new string[] { "Password" },
    RegexValuesToRedact = new string[] { "Bearer" }
};

var options = Options.Create(redactorServiceOptions);
var redactorService = new RedactorService(options);

//ICorrelationService
var correlationService = new CorrelationService();

//ILoggingService

var telemetrySQLServiceLoggerService = new LoggingService<TelemetrySQLService>(loggerFactory.CreateLogger<TelemetrySQLService>(), redactorService, correlationService);
var telemetryServiceLoggerService = new LoggingService<TelemetryService>(loggerFactory.CreateLogger<TelemetryService>(), redactorService, correlationService);


//ITelemetrySQLService
var telemetrySQLService = new TelemetrySQLService(telemetrySQLServiceLoggerService);

//ITelemetryDBService
var telemetryDBService = new TelemetryDBService(options, telemetrySQLService)

//TelemetryService
var telemetryService = new TelemetryService(options, applicationLifetime, telemetryDBService, loggerFactory.CreateLogger<TelemetryService>());

```

<h3>Create Instance (With Dependency Injection)</h3>

<h4>Add appsettings.json File With Contents</h4>

Note: Runner Shows this with added steps to enypct Connection String

 ```json  
{
  "TelemetryServiceOptions": {
    "Source": "DickinsonBros.Telemtry.Runner",
    "RecordDurableRest": true,
    "RecordSQL": true,
    "RecordQueue": true,
    "RecordAPI": true,
    "RecordEmail": true,
    "ConnectionString": "..."
  }
}
 ```    
<h4>Code</h4>

```c#

var serviceCollection = new ServiceCollection();

//Configure Options
var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false)

var configuration = builder.Build();
serviceCollection.AddOptions();
services.Configure<CertificateEncryptionServiceOptions<RunnerCertificateEncryptionServiceOptions>>(_configuration.GetSection(nameof(RunnerCertificateEncryptionServiceOptions)));

//Add Service
services.AddCertificateEncryptionService<RunnerCertificateEncryptionServiceOptions>();

//Build Service Provider 
using (var provider = services.BuildServiceProvider())
{
  var certificateEncryptionService = provider.GetRequiredService<ICertificateEncryptionService<RunnerCertificateEncryptionServiceOptions>>();
}
```

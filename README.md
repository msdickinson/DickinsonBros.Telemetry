# DickinsonBros.Telemetry

<a href="https://dev.azure.com/marksamdickinson/dickinsonbros/_build/latest?definitionId=53&amp;branchName=master"> <img alt="Azure DevOps builds (branch)" src="https://img.shields.io/azure-devops/build/marksamdickinson/DickinsonBros/53/master"> </a> <a href="https://dev.azure.com/marksamdickinson/dickinsonbros/_build/latest?definitionId=53&amp;branchName=master"> <img alt="Azure DevOps coverage (branch)" src="https://img.shields.io/azure-devops/coverage/marksamdickinson/dickinsonbros/53/master"> </a><a href="https://dev.azure.com/marksamdickinson/DickinsonBros/_release?_a=releases&view=mine&definitionId=25"> <img alt="Azure DevOps releases" src="https://img.shields.io/azure-devops/release/marksamdickinson/b5a46403-83bb-4d18-987f-81b0483ef43e/25/26"> </a><a href="https://www.nuget.org/packages/DickinsonBros.Telemetry/"><img src="https://img.shields.io/nuget/v/DickinsonBros.Telemetry"></a>

A Telemetry service

      Features
      * Vaildates telemetry
      * Sends telemetry event

<h2>Example Usage</h2>

```C#
    //(Subscriber) designed for other packages to use as a sink
    telemetryService.NewTelemetryEvent += (telemetryData) => {
      Console.WriteLine("NewTelemetryEvent");
      Console.WriteLine(JsonSerializer.Serialize(telemetryData));
      Console.WriteLine();
    };
    
    //(Publisher) Sends telemetry event
    Console.WriteLine("Insert API Telemetry (50 Times)");
    for (int i = 0; i < 50; i++)
    {
        telemetryService.Insert(GenerateTelemetry());
    }
                    
```

[Sample Runner](/Runner/DickinsonBros.Telemtry.Runner)

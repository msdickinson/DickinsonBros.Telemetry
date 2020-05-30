using DickinsonBros.Encryption.Certificate.Extensions;
using DickinsonBros.Encryption.Certificate.Models;
using DickinsonBros.Logger.Extensions;
using DickinsonBros.Redactor.Extensions;
using DickinsonBros.Redactor.Models;
using DickinsonBros.Telemetry.Abstractions;
using DickinsonBros.Telemetry.Abstractions.Models;
using DickinsonBros.Telemetry.Extensions;
using DickinsonBros.Telemetry.Models;
using DickinsonBros.Telemetry.Runner.Models;
using DickinsonBros.Telemetry.Runner.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DickinsonBros.Telemetry.Runner
{
    class Program
    {
        IConfiguration _configuration;
        async static Task Main()
        {
            await new Program().DoMain();
        }
        async Task DoMain()
        {
            try
            {
                using var applicationLifetime = new ApplicationLifetime();
                var services = InitializeDependencyInjection();
                ConfigureServices(services, applicationLifetime);

                using (var provider = services.BuildServiceProvider())
                {
                    var telemetryService = provider.GetRequiredService<ITelemetryService>();
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
                }

                applicationLifetime.StopApplication();
                await Task.CompletedTask.ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Console.WriteLine("End...");
            }
        }

        private SQLTelemetry GenerateSQLTelemetry()
        {
            return new SQLTelemetry
            {
                CorrelationId = Guid.NewGuid().ToString(),
                Database = "DickinsonBros",
                RequestRedacted =
@"{
    ""username"":""NewUser"",   
    ""passwordhash"":""***Redacted***"",
    ""salt"":""***Redacted***"",
    ""email"":""marktest@gmail.com"",
    ""emailPreference"": 1,
    ""emailPreferenceToken"":""***Redacted***"",
    ""activateEmailToken"":""***Redacted***"",
}",

                ElapsedMilliseconds = new Random().Next(0, 1000),
                Query = "[Account].[Insert]",

                IsSuccessful = true,
                Source = "Runner.Queue"
            };
        }

        private QueueTelemetry GenerateQueueTelemetry()
        {
            return new QueueTelemetry
            {
                CorrelationId = Guid.NewGuid().ToString(),
                ElapsedMilliseconds = new Random().Next(0, 1000),
                Name = "AccountPostSetup",
                QueueId = 1,
                State = QueueState.Ready,
                IsSuccessful = true,
                Source = "Runner.Queue"
            };
        }

        private EmailTelemetry GenerateEmailTelemetry()
        {
            return new EmailTelemetry
            {
                CorrelationId = Guid.NewGuid().ToString(),
                ElapsedMilliseconds = new Random().Next(0, 1000),
                IsSuccessful = true,
                Source = "Runner.Email",
                Subject = "Roller Coaster Maker - Activate Your Email",
                To = "marktest@gmail.com",
            };
        }

        private DurableRestTelemetry GenerateDurableRestTelemetry()
        {
            return new DurableRestTelemetry
            {
                Attempt = 1,
                BaseUrl = "https://www.RollerCoasterMaker.com/AccountAPI/",
                CorrelationId = "",
                ElapsedMilliseconds = new Random().Next(0, 1000),
                Name = "Create Account",
                RequestRedacted =
 @"{
    ""Username"":""NewUser"",   
    ""Password"":""***Redacted***""  
}",
                Resource = "CreateAccount",
                ResponseRedacted =
 @"{
    ""Token"":""***Redacted***""  
}",
                StatusCode = 201,
                Source = "Runner.Proxy"
            };
        }

        private APITelemetry GenerateAPITelemetry()
        {
            return new APITelemetry
            {
                CorrelationId = Guid.NewGuid().ToString(),
                ElapsedMilliseconds = new Random().Next(0, 1000),
                Source = "Account API",
                RequestRedacted =
 @"{
    ""Username"":""NewUser"",   
    ""Password"":""***Redacted***""  
}",
                ResponseRedacted =
 @"{
    ""Token"":""***Redacted***""  
}",
                StatusCode = 201,
                Url = "https://www.RollerCoasterMaker.com/AccountAPI/CreateAccount"
            };
        }

        private void ConfigureServices(IServiceCollection services, ApplicationLifetime applicationLifetime)
        {
            services.AddOptions();
            services.AddLogging(config =>
            {
                config.AddConfiguration(_configuration.GetSection("Logging"));

                if (Environment.GetEnvironmentVariable("BUILD_CONFIGURATION") == "DEBUG")
                {
                    config.AddConsole();
                }
            });
            services.AddSingleton<IApplicationLifetime>(applicationLifetime);

            //Add Logging Service
            services.AddLoggingService();

            //Add Redactor
            services.AddRedactorService();
            services.Configure<RedactorServiceOptions>(_configuration.GetSection(nameof(RedactorServiceOptions)));

            //Add Certificate Encryption
            services.AddCertificateEncryptionService<CertificateEncryptionServiceOptions>();
            services.Configure<CertificateEncryptionServiceOptions<RunnerCertificateEncryptionServiceOptions>>(_configuration.GetSection(nameof(RunnerCertificateEncryptionServiceOptions)));

            //Add Telemetry
            services.AddTelemetryService();
            services.AddSingleton<IConfigureOptions<TelemetryServiceOptions>, TelemetryServiceOptionsOptionsConfigurator>();
        }

        IServiceCollection InitializeDependencyInjection()
        {
            var aspnetCoreEnvironment = Environment.GetEnvironmentVariable("BUILD_CONFIGURATION");
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", true);
            _configuration = builder.Build();
            var services = new ServiceCollection();
            services.AddSingleton(_configuration);
            return services;
        }
    }
}


using DickinsonBros.Encryption.Certificate.Extensions;
using DickinsonBros.Logger.Extensions;
using DickinsonBros.Redactor.Extensions;
using DickinsonBros.Telemetry.Abstractions;
using DickinsonBros.Telemetry.Abstractions.Models;
using DickinsonBros.Telemetry.Extensions;
using DickinsonBros.Telemetry.Runner.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
                var services = InitializeDependencyInjection();
                ConfigureServices(services);

                using (var provider = services.BuildServiceProvider())
                {
                    var telemetryService = provider.GetRequiredService<ITelemetryService>();
                    var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();
                    Console.WriteLine("Insert API Telemetry (50 Times)");
                    for (int i = 0; i < 50; i++)
                    {
                        telemetryService.Insert(GenerateTelemetry());
                    }

                    Console.WriteLine("Flush Telemetry");

                    await telemetryService.FlushAsync().ConfigureAwait(false);

                    hostApplicationLifetime.StopApplication();
                }

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

        private TelemetryData GenerateTelemetry()
        {
            return new TelemetryData
            {
                Name = new Random().Next(0, 5).ToString(),
                ElapsedMilliseconds = new Random().Next(0, 1000),
                TelemetryState = (TelemetryState)Enum.ToObject(typeof(TelemetryState), new Random().Next(0, 2)),
                TelemetryType = (TelemetryType)Enum.ToObject(typeof(TelemetryType), new Random().Next(0, 3)),
                DateTime = DateTime.UtcNow
            };
        }

        private void ConfigureServices(IServiceCollection services)
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

            services.AddSingleton<IHostApplicationLifetime, HostApplicationLifetime>();

            //Add Logging Service
            services.AddLoggingService();

            //Add Redactor
            services.AddRedactorService();

            //Add Certificate Encryption
            services.AddConfigurationEncryptionService();

            //Add Telemetry
            services.AddTelemetryService();
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


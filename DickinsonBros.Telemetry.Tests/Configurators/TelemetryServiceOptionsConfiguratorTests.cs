using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.Telemetry.Configurators;
using DickinsonBros.Telemetry.Models;
using DickinsonBros.Test;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace DickinsonBros.Telemetry.Tests.Configurators
{
    [TestClass]
    public class TelemetryServiceOptionsConfiguratorTests : BaseTest
    {

        [TestMethod]
        public async Task Configure_Runs_ConfigReturns()
        {
            var telemetryServiceOptions = new TelemetryServiceOptions
            {
                ConnectionString = "SampleConnectionString",
                Source = "SampleSource"
            };

            var telemetryServiceOptionsDecrypted = new TelemetryServiceOptions
            {
                ConnectionString = "SampleDecryptedConnectionString",
                Source = "SampleDecryptedSource"
            };

            var configurationRoot = BuildConfigurationRoot(telemetryServiceOptions);

            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var configurationEncryptionServiceMock = serviceProvider.GetMock<IConfigurationEncryptionService>();

                    configurationEncryptionServiceMock
                    .Setup
                    (
                        configurationEncryptionService => configurationEncryptionService.Decrypt
                        (
                            telemetryServiceOptions.ConnectionString
                        )
                    )
                    .Returns
                    (
                            telemetryServiceOptionsDecrypted.ConnectionString
                    );

                    //Act
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>().Value;

                    //Assert
                    Assert.IsNotNull(options);

                    Assert.AreEqual(telemetryServiceOptionsDecrypted.ConnectionString, options.ConnectionString);
                    Assert.AreEqual(telemetryServiceOptions.Source, options.Source);

                    await Task.CompletedTask.ConfigureAwait(false);

                },
                serviceCollection => ConfigureServices(serviceCollection, configurationRoot)
            );
        }

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddOptions();
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddSingleton<IConfigureOptions<TelemetryServiceOptions>, TelemetryServiceOptionsConfigurator>();
            serviceCollection.AddSingleton(Mock.Of<IConfigurationEncryptionService>());

            return serviceCollection;
        }

        #endregion
    }
}

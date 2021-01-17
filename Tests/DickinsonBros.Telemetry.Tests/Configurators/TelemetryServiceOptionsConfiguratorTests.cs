using DickinsonBros.Telemetry.Configurators;
using DickinsonBros.Telemetry.Models;
using DickinsonBros.Test;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                Source = "SampleSource"
            };

            var configurationRoot = BuildConfigurationRoot(telemetryServiceOptions);

            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                  
                    //Act
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>().Value;

                    //Assert
                    Assert.IsNotNull(options);
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

            return serviceCollection;
        }

        #endregion
    }
}

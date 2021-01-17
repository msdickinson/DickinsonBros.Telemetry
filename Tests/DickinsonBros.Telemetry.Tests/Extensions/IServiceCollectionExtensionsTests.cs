using DickinsonBros.Telemetry.Abstractions;
using DickinsonBros.Telemetry.Extensions;
using DickinsonBros.Telemetry.Services.SQL;
using DickinsonBros.Telemetry.Services.TelemetryDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DickinsonBros.Telemetry.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void AddTelemetryService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddTelemetryService();

            // Assert

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(ITelemetryService) &&
                                           serviceDefinition.ImplementationType == typeof(TelemetryService) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(ITelemetrySQLService) &&
                               serviceDefinition.ImplementationType == typeof(TelemetrySQLService) &&
                               serviceDefinition.Lifetime == ServiceLifetime.Singleton));

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(ITelemetryDBService) &&
                               serviceDefinition.ImplementationType == typeof(TelemetryDBService) &&
                               serviceDefinition.Lifetime == ServiceLifetime.Singleton));

        }
    }
}

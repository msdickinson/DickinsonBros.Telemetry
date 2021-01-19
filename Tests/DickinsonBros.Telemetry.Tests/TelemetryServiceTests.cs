using DickinsonBros.Telemetry.Abstractions;
using DickinsonBros.Telemetry.Abstractions.Models;
using DickinsonBros.Telemetry.Models;
using DickinsonBros.UnitTest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace DickinsonBros.Telemetry.Tests
{
    [TestClass]
    public class TelemetryServiceTests : BaseTest
    {
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Insert_InputTelemetryIsNull_ThrowsNullException()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var telemetryDataPassedIn = (TelemetryData)null;

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    uutConcrete.Insert(telemetryDataPassedIn);
                  
                    //Assert
                    
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Insert_InputNameIsEmpty_ThrowsNullException()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var telemetryDataPassedIn = new TelemetryData
                    {
                        Name = ""
                    };

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    uutConcrete.Insert(telemetryDataPassedIn);

                    //Assert

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "")]
        public void Insert_InputDateTimeIsDefault_ThrowsNullException()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var telemetryDataPassedIn = new TelemetryData
                    {
                        Name = "abc"
                    };

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    uutConcrete.Insert(telemetryDataPassedIn);

                    //Assert

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void Insert_URIWithParameters_EnqueueEmailTelemetry()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();

                    var uriNameWithPrams = "https://www.samplerequset.com/api/getUser?id=5";
                    var uriNameWithOutPrams = "https://www.samplerequset.com/api/getUser";
                    var telemetryDataExpected = new TelemetryData
                    {
                        Name = uriNameWithPrams,
                        DateTime = new DateTime(2020, 6, 3)
                    };

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;
                    var telemetryDataObserved = (TelemetryData)null;
                    uut.NewTelemetryEvent += (telemetryData) => {
                        telemetryDataObserved = telemetryData;
                    };

                    //Act
                    uutConcrete.Insert(telemetryDataExpected);

                    //Assert
                    Assert.AreEqual(telemetryDataExpected, telemetryDataObserved);
                    Assert.AreEqual(uriNameWithOutPrams, telemetryDataExpected.Name);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void Insert_VaildInput_EnqueueEmailTelemetry()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();

                    var telemetryDataExpected = new TelemetryData
                    {
                        Name = "name",
                        DateTime = new DateTime(2020, 6, 3)
                    };

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;
                    var telemetryDataObserved = (TelemetryData)null;
                    uut.NewTelemetryEvent += (telemetryData) => {
                        telemetryDataObserved = telemetryData;
                    };

                    //Act
                    uutConcrete.Insert(telemetryDataExpected);
                    Assert.AreEqual(telemetryDataExpected, telemetryDataObserved);
                    Assert.AreEqual(telemetryDataExpected, telemetryDataObserved);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ITelemetryService, TelemetryService>();

            var telemetryDBOptions = new TelemetryServiceOptions
            {
                Source = "Source"
            };

            var options = Options.Create(telemetryDBOptions);
            serviceCollection.AddSingleton<IOptions<TelemetryServiceOptions>>(options);
            serviceCollection.AddSingleton(Mock.Of<IHostApplicationLifetime>());

            return serviceCollection;
        }

    }
}

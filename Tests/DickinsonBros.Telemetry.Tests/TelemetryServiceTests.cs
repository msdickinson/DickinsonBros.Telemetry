using DickinsonBros.Logger.Abstractions;
using DickinsonBros.Telemetry.Abstractions;
using DickinsonBros.Telemetry.Abstractions.Models;
using DickinsonBros.Telemetry.Models;
using DickinsonBros.Telemetry.Services.TelemetryDB;
using DickinsonBros.Test;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

                    //Act
                    uutConcrete.Insert(telemetryDataExpected);

                    //Assert
                    uutConcrete._queueTelemetry.TryPeek(out TelemetryData etelemetryObserved);
                    Assert.AreEqual(1, uutConcrete._queueTelemetry.Count);
                    Assert.AreEqual(telemetryDataExpected, etelemetryObserved);
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

                    //Act
                    uutConcrete.Insert(telemetryDataExpected);

                    //Assert
                    uutConcrete._queueTelemetry.TryPeek(out TelemetryData etelemetryObserved);
                    Assert.AreEqual(1, uutConcrete._queueTelemetry.Count);
                    Assert.AreEqual(telemetryDataExpected, etelemetryObserved);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task Upload_QueueTelemetryHasAValueAndRecordQueueIsTrue_EnqueueQueueTelemetry()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();


                    var telemetryExpected = new TelemetryData();
                    var telemetryObserved = (List<TelemetryData>)null;
                    var telemetryDBServiceMock = serviceProvider.GetMock<ITelemetryDBService>();

                    //API Call Back
                    telemetryDBServiceMock
                    .Setup
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertTelemetryAsync
                        (
                            It.IsAny<List<TelemetryData>>()
                         )
                    )
                    .Callback<List<TelemetryData>>((apiTelemetry) =>
                    {
                        telemetryObserved = apiTelemetry;
                    });

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    uutConcrete._queueTelemetry.Enqueue(telemetryExpected);

                    //Act
                    await uutConcrete.Upload();

                    //Assert
                    Assert.AreEqual(0, uutConcrete._queueTelemetry.Count);


                    Assert.AreEqual(telemetryExpected, telemetryObserved.First());
   

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task Uploader_CancellationIsRequestedBeforeCall_BulkInsertsNotCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();

                    var cancellationTokenSource  = new CancellationTokenSource();
                    var token = cancellationTokenSource.Token;
                    cancellationTokenSource.Cancel();

                    var telemetryData = new TelemetryData();

                    var telemetryDBServiceMock = serviceProvider.GetMock<ITelemetryDBService>();

                    //API Call Back
                    telemetryDBServiceMock
                    .Setup
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertTelemetryAsync
                        (
                            It.IsAny<List<TelemetryData>>()
                        )
                    );

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    uutConcrete._queueTelemetry.Enqueue(telemetryData);

                    //Act
                    await uutConcrete.Uploader(token);

                    //Assert
                    Assert.AreEqual(1, uutConcrete._queueTelemetry.Count);

                    telemetryDBServiceMock
                    .Verify
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertTelemetryAsync
                        (
                            It.IsAny<List<TelemetryData>>()
                        ),
                        Times.Never
                    );


                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task Uploader_CancellationIsRequestedAfterCall_BulkInsertsCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();

                    var cancellationTokenSource = new CancellationTokenSource();
                    var token = cancellationTokenSource.Token;

                    var telemetryData = new TelemetryData();
                    var telemetryDBServiceMock = serviceProvider.GetMock<ITelemetryDBService>();

                    //API Call Back
                    telemetryDBServiceMock
                    .Setup
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertTelemetryAsync
                        (
                            It.IsAny<List<TelemetryData>>()
                         )
                    );

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    uutConcrete._queueTelemetry.Enqueue(telemetryData);

                    //Act
                    var task = uutConcrete.Uploader(token);

                    await Task.Delay(5000).ConfigureAwait(false);
                    cancellationTokenSource.Cancel();
                    await task.ConfigureAwait(false);

                    //Assert
                    Assert.AreEqual(0, uutConcrete._queueTelemetry.Count);

                    telemetryDBServiceMock
                    .Verify
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertTelemetryAsync
                        (
                            It.IsAny<List<TelemetryData>>()
                        ),
                        Times.AtLeastOnce
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task Uploader_Throws_ExceptionCaughtAndLogged()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();


                    var cancellationTokenSource = new CancellationTokenSource();
                    var token = cancellationTokenSource.Token;

                    var telemetryData = new TelemetryData();

                    var telemetryDBServiceMock = serviceProvider.GetMock<ITelemetryDBService>();

                    //API Call Back
                    telemetryDBServiceMock
                    .Setup
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertTelemetryAsync
                        (
                            It.IsAny<List<TelemetryData>>()
                        )
                    ).ThrowsAsync(new System.Exception());

                    //  Logging
                    var messageObserved = (string)null;
                    var exceptionObserved = (Exception)null;
                    var propertiesObserved = (Dictionary<string, object>)null;

                    var loggingServiceMock = serviceProvider.GetMock<ILoggingService<TelemetryService>>();
                    loggingServiceMock
                        .Setup
                        (
                            loggingService => loggingService.LogErrorRedacted
                            (
                                It.IsAny<string>(),
                                It.IsAny<Exception>(),
                                It.IsAny<IDictionary<string, object>>()
                            )
                        )
                        .Callback<string, Exception, IDictionary<string, object>>((message, exception, properties) =>
                        {
                            messageObserved = message;
                            exceptionObserved = exception;
                            propertiesObserved = (Dictionary<string, object>)properties;
                        });

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    uutConcrete._queueTelemetry.Enqueue(telemetryData);

                    //Act
                    var task = uutConcrete.Uploader(token);
                    await Task.Delay(5000).ConfigureAwait(false);
                    cancellationTokenSource.Cancel();
                    await task.ConfigureAwait(false);

                    //Assert

                    telemetryDBServiceMock
                    .Verify
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertTelemetryAsync
                        (
                            It.IsAny<List<TelemetryData>>()
                        ),
                        Times.AtLeastOnce()
                    );

                    Assert.IsTrue(exceptionObserved != null);

                    loggingServiceMock.Verify
                    (
                        loggingService => loggingService.LogErrorRedacted
                        (
                            messageObserved,
                            exceptionObserved,
                            propertiesObserved
                        )
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task Flush_TokenCanceled_RunsSuccessfully()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();

                    var cancellationTokenSource = new CancellationTokenSource();
                    var token = cancellationTokenSource.Token;

                    var telemetryDBServiceMock = serviceProvider.GetMock<ITelemetryDBService>();
                    var hostApplicationLifetime = serviceProvider.GetMock<IHostApplicationLifetime>();

                    hostApplicationLifetime
                        .SetupGet(applicationLifetime => applicationLifetime.ApplicationStopping)
                        .Returns(token);

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    cancellationTokenSource.Cancel();
                    await uutConcrete.FlushAsync().ConfigureAwait(false);

                    //Assert

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task Flush_TokenNotCanceled_RunsSuccessfully()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();

                    var cancellationTokenSource = new CancellationTokenSource();
                    var token = cancellationTokenSource.Token;

                    var telemetryDBServiceMock = serviceProvider.GetMock<ITelemetryDBService>();
                    var hostApplicationLifetime = serviceProvider.GetMock<IHostApplicationLifetime>();

                    hostApplicationLifetime
                        .SetupGet(applicationLifetime => applicationLifetime.ApplicationStopping)
                        .Returns(token);

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    await uutConcrete.FlushAsync().ConfigureAwait(false);

                    //Assert

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ITelemetryService, TelemetryService>();

            var telemetryDBOptions = new TelemetryServiceOptions
            {
                ConnectionString = "ConnectionString"
            };

            var options = Options.Create(telemetryDBOptions);
            serviceCollection.AddSingleton<IOptions<TelemetryServiceOptions>>(options);
            serviceCollection.AddSingleton(Mock.Of<IHostApplicationLifetime>());
            serviceCollection.AddSingleton(Mock.Of<ITelemetryDBService>());
            serviceCollection.AddSingleton(Mock.Of<ILoggingService<TelemetryService>>());

            return serviceCollection;
        }

    }
}

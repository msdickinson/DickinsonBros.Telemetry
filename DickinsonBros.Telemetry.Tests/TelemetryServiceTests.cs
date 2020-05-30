using DickinsonBros.Logger.Abstractions;
using DickinsonBros.Telemetry.Abstractions;
using DickinsonBros.Telemetry.Abstractions.Models;
using DickinsonBros.Telemetry.Models;
using DickinsonBros.Telemetry.Services.TelemetryDB;
using DickinsonBros.Test;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
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
        public void InsertEmail_RecordEmailIsFalse_DoNotEnqueueEmailTelemetry()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();
                    options.Value.RecordEmail = false;

                    var emailTelemetryPassedIn = new EmailTelemetry();

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    uutConcrete.InsertEmail(emailTelemetryPassedIn);

                    //Assert
                    Assert.AreEqual(0, uutConcrete._queueEmailTelemetry.Count);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void InsertEmail_InputEmailTelemetryIsNull_DoNotEnqueueEmailTelemetry()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();
                    options.Value.RecordEmail = true;

                    var emailTelemetryPassedIn = (EmailTelemetry)null;

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    uutConcrete.InsertEmail(emailTelemetryPassedIn);

                    //Assert
                    Assert.AreEqual(0, uutConcrete._queueEmailTelemetry.Count);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void InsertEmail_EmailTelemetryHasAValueAndRecordEmailIsTrue_EnqueueEmailTelemetry()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();
                    options.Value.RecordEmail = true;

                    var emailTelemetryExpected = new EmailTelemetry();

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    uutConcrete.InsertEmail(emailTelemetryExpected);

                    //Assert
                    uutConcrete._queueEmailTelemetry.TryPeek(out EmailTelemetry emailTelemetryObserved);
                    Assert.AreEqual(1, uutConcrete._queueEmailTelemetry.Count);
                    Assert.AreEqual(emailTelemetryExpected, emailTelemetryObserved);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void InsertSQL_RecordSQLIsFalse_DoNotEnqueueSQLTelemetry()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();
                    options.Value.RecordSQL = false;

                    var sqlTelemetryPassedIn = new SQLTelemetry();

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    uutConcrete.InsertSQL(sqlTelemetryPassedIn);

                    //Assert
                    Assert.AreEqual(0, uutConcrete._queueSQLTelemetry.Count);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void InsertSQL_InputSQLTelemetryIsNull_DoNotEnqueueSQLTelemetry()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();
                    options.Value.RecordSQL = true;

                    var sqlTelemetryPassedIn = (SQLTelemetry)null;

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    uutConcrete.InsertSQL(sqlTelemetryPassedIn);

                    //Assert
                    Assert.AreEqual(0, uutConcrete._queueSQLTelemetry.Count);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void InsertSQL_SQLTelemetryHasAValueAndRecordSQLIsTrue_EnqueueSQLTelemetry()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();
                    options.Value.RecordSQL = true;

                    var sqlTelemetryExpected = new SQLTelemetry();

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    uutConcrete.InsertSQL(sqlTelemetryExpected);

                    //Assert
                    uutConcrete._queueSQLTelemetry.TryPeek(out SQLTelemetry sqlTelemetryObserved);
                    Assert.AreEqual(1, uutConcrete._queueSQLTelemetry.Count);
                    Assert.AreEqual(sqlTelemetryExpected, sqlTelemetryObserved);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void InsertAPI_RecordAPIIsFalse_DoNotEnqueueAPITelemetry()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();
                    options.Value.RecordAPI = false;

                    var apiTelemetryPassedIn = new APITelemetry();

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    uutConcrete.InsertAPI(apiTelemetryPassedIn);

                    //Assert
                    Assert.AreEqual(0, uutConcrete._queueAPITelemetry.Count);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void InsertAPI_InputAPITelemetryIsNull_DoNotEnqueueAPITelemetry()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();
                    options.Value.RecordAPI = true;

                    var apiTelemetryPassedIn = (APITelemetry)null;

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    uutConcrete.InsertAPI(apiTelemetryPassedIn);

                    //Assert
                    Assert.AreEqual(0, uutConcrete._queueAPITelemetry.Count);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void InsertAPI_APITelemetryHasAValueAndRecordAPIIsTrue_EnqueueAPITelemetry()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();
                    options.Value.RecordAPI = true;

                    var apiTelemetryExpected = new APITelemetry();

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    uutConcrete.InsertAPI(apiTelemetryExpected);

                    //Assert
                    uutConcrete._queueAPITelemetry.TryPeek(out APITelemetry apiTelemetryObserved);
                    Assert.AreEqual(1, uutConcrete._queueAPITelemetry.Count);
                    Assert.AreEqual(apiTelemetryExpected, apiTelemetryObserved);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void InsertDurableRest_RecordDurableRestIsFalse_DoNotEnqueueDurableRestTelemetry()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();
                    options.Value.RecordDurableRest = false;

                    var durableRestTelemetryPassedIn = new DurableRestTelemetry();

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    uutConcrete.InsertDurableRest(durableRestTelemetryPassedIn);

                    //Assert
                    Assert.AreEqual(0, uutConcrete._queueDurableRestTelemetry.Count);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void InsertDurableRest_InputDurableRestTelemetryIsNull_DoNotEnqueueDurableRestTelemetry()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();
                    options.Value.RecordDurableRest = true;

                    var durableRestTelemetryPassedIn = (DurableRestTelemetry)null;

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    uutConcrete.InsertDurableRest(durableRestTelemetryPassedIn);

                    //Assert
                    Assert.AreEqual(0, uutConcrete._queueDurableRestTelemetry.Count);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void InsertDurableRest_DurableRestTelemetryHasAValueAndRecordDurableRestIsTrue_EnqueueDurableRestTelemetry()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();
                    options.Value.RecordDurableRest = true;

                    var durableRestTelemetryExpected = new DurableRestTelemetry();

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    uutConcrete.InsertDurableRest(durableRestTelemetryExpected);

                    //Assert
                    uutConcrete._queueDurableRestTelemetry.TryPeek(out DurableRestTelemetry durableRestTelemetryObserved);
                    Assert.AreEqual(1, uutConcrete._queueDurableRestTelemetry.Count);
                    Assert.AreEqual(durableRestTelemetryExpected, durableRestTelemetryObserved);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void InsertQueue_RecordQueueIsFalse_DoNotEnqueueQueueTelemetry()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();
                    options.Value.RecordQueue = false;

                    var queueTelemetryPassedIn = new QueueTelemetry();

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    uutConcrete.InsertQueue(queueTelemetryPassedIn);

                    //Assert
                    Assert.AreEqual(0, uutConcrete._queueQueueTelemetry.Count);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void InsertQueue_InputQueueTelemetryIsNull_DoNotEnqueueQueueTelemetry()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();
                    options.Value.RecordQueue = true;

                    var queueTelemetryPassedIn = (QueueTelemetry)null;

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    uutConcrete.InsertQueue(queueTelemetryPassedIn);

                    //Assert
                    Assert.AreEqual(0, uutConcrete._queueQueueTelemetry.Count);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void InsertQueue_QueueTelemetryHasAValueAndRecordQueueIsTrue_EnqueueQueueTelemetry()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryServiceOptions>>();
                    options.Value.RecordQueue = true;

                    var queueTelemetryExpected = new QueueTelemetry();

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    uutConcrete.InsertQueue(queueTelemetryExpected);

                    //Assert
                    uutConcrete._queueQueueTelemetry.TryPeek(out QueueTelemetry queueTelemetryObserved);
                    Assert.AreEqual(1, uutConcrete._queueQueueTelemetry.Count);
                    Assert.AreEqual(queueTelemetryExpected, queueTelemetryObserved);
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

                    var apiTelemetryExpected            = new APITelemetry();
                    var durableRestTelemetryExpected    = new DurableRestTelemetry();
                    var emailTelemetryExpected          = new EmailTelemetry();
                    var queueTelemetryExpected          = new QueueTelemetry();
                    var sqlTelemetryExpected            = new SQLTelemetry();

                    var apiTelemetryObserved = (List<APITelemetry>)null;
                    var durableRestTelemetryObserved = (List<DurableRestTelemetry>)null;
                    var emailTelemetryObserved = (List<EmailTelemetry>)null;
                    var queueTelemetryObserved = (List<QueueTelemetry>)null;
                    var sqlTelemetryObserved = (List<SQLTelemetry>)null;

                    var telemetryDBServiceMock = serviceProvider.GetMock<ITelemetryDBService>();

                    //API Call Back
                    telemetryDBServiceMock
                    .Setup
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertAPITelemetryAsync
                        (
                            It.IsAny<List<APITelemetry>>()
                         )
                    )
                    .Callback<List<APITelemetry>>((apiTelemetry) =>
                    {
                        apiTelemetryObserved = apiTelemetry;
                    });

                    //DurableRest Call Back
                    telemetryDBServiceMock
                    .Setup
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertDurableRestTelemetryAsync
                        (
                            It.IsAny<List<DurableRestTelemetry>>()
                         )
                    )
                    .Callback<List<DurableRestTelemetry>>((durableRestTelemetry) =>
                    {
                        durableRestTelemetryObserved = durableRestTelemetry;
                    });

                    //Email Call Back
                    telemetryDBServiceMock
                    .Setup
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertEmailTelemetryAsync
                        (
                            It.IsAny<List<EmailTelemetry>>()
                         )
                    )
                    .Callback<List<EmailTelemetry>>((emailTelemetry) =>
                    {
                        emailTelemetryObserved = emailTelemetry;
                    });

                    //Queue Call Back
                    telemetryDBServiceMock
                    .Setup
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertQueueTelemetryAsync
                        (
                            It.IsAny<List<QueueTelemetry>>()
                         )
                    )
                    .Callback<List<QueueTelemetry>>((queueTelemetry) =>
                    {
                        queueTelemetryObserved = queueTelemetry;
                    });

                    //SQL Call Back
                    telemetryDBServiceMock
                    .Setup
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertSQLTelemetryAsync
                        (
                            It.IsAny<List<SQLTelemetry>>()
                         )
                    )
                    .Callback<List<SQLTelemetry>>((sqlTelemetry) =>
                    {
                        sqlTelemetryObserved = sqlTelemetry;
                    });

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    uutConcrete._queueAPITelemetry.Enqueue(apiTelemetryExpected);
                    uutConcrete._queueDurableRestTelemetry.Enqueue(durableRestTelemetryExpected);
                    uutConcrete._queueEmailTelemetry.Enqueue(emailTelemetryExpected);
                    uutConcrete._queueQueueTelemetry.Enqueue(queueTelemetryExpected);
                    uutConcrete._queueSQLTelemetry.Enqueue(sqlTelemetryExpected);

                    //Act
                    await uutConcrete.Upload();

                    //Assert
                    Assert.AreEqual(0, uutConcrete._queueQueueTelemetry.Count);
                    Assert.AreEqual(0, uutConcrete._queueDurableRestTelemetry.Count);
                    Assert.AreEqual(0, uutConcrete._queueEmailTelemetry.Count);
                    Assert.AreEqual(0, uutConcrete._queueQueueTelemetry.Count);
                    Assert.AreEqual(0, uutConcrete._queueSQLTelemetry.Count);

                    Assert.AreEqual(apiTelemetryExpected, apiTelemetryObserved.First());
                    Assert.AreEqual(durableRestTelemetryExpected, durableRestTelemetryObserved.First());
                    Assert.AreEqual(emailTelemetryExpected, emailTelemetryObserved.First());
                    Assert.AreEqual(queueTelemetryExpected, queueTelemetryObserved.First());
                    Assert.AreEqual(sqlTelemetryExpected, sqlTelemetryObserved.First());

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

                    var apiTelemetry = new APITelemetry();
                    var durableRestTelemetry = new DurableRestTelemetry();
                    var emailTelemetry = new EmailTelemetry();
                    var queueTelemetry = new QueueTelemetry();
                    var sqlTelemetry = new SQLTelemetry();

                    var telemetryDBServiceMock = serviceProvider.GetMock<ITelemetryDBService>();

                    //API Call Back
                    telemetryDBServiceMock
                    .Setup
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertAPITelemetryAsync
                        (
                            It.IsAny<List<APITelemetry>>()
                         )
                    );

                    //DurableRest Call Back
                    telemetryDBServiceMock
                    .Setup
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertDurableRestTelemetryAsync
                        (
                            It.IsAny<List<DurableRestTelemetry>>()
                         )
                    );

                    //Email Call Back
                    telemetryDBServiceMock
                    .Setup
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertEmailTelemetryAsync
                        (
                            It.IsAny<List<EmailTelemetry>>()
                         )
                    );

                    //Queue Call Back
                    telemetryDBServiceMock
                    .Setup
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertQueueTelemetryAsync
                        (
                            It.IsAny<List<QueueTelemetry>>()
                         )
                    );

                    //SQL Call Back
                    telemetryDBServiceMock
                    .Setup
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertSQLTelemetryAsync
                        (
                            It.IsAny<List<SQLTelemetry>>()
                         )
                    );

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    uutConcrete._queueAPITelemetry.Enqueue(apiTelemetry);
                    uutConcrete._queueDurableRestTelemetry.Enqueue(durableRestTelemetry);
                    uutConcrete._queueEmailTelemetry.Enqueue(emailTelemetry);
                    uutConcrete._queueQueueTelemetry.Enqueue(queueTelemetry);
                    uutConcrete._queueSQLTelemetry.Enqueue(sqlTelemetry);

                    //Act
                    await uutConcrete.Uploader(token);

                    //Assert
                    Assert.AreEqual(1, uutConcrete._queueQueueTelemetry.Count);
                    Assert.AreEqual(1, uutConcrete._queueDurableRestTelemetry.Count);
                    Assert.AreEqual(1, uutConcrete._queueEmailTelemetry.Count);
                    Assert.AreEqual(1, uutConcrete._queueQueueTelemetry.Count);
                    Assert.AreEqual(1, uutConcrete._queueSQLTelemetry.Count);

                    telemetryDBServiceMock
                    .Verify
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertAPITelemetryAsync
                        (
                            It.IsAny<List<APITelemetry>>()
                        ),
                        Times.Never
                    );

                    telemetryDBServiceMock
                    .Verify
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertDurableRestTelemetryAsync
                        (
                            It.IsAny<List<DurableRestTelemetry>>()
                        ),
                        Times.Never
                    );

                    telemetryDBServiceMock
                    .Verify
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertEmailTelemetryAsync
                        (
                            It.IsAny<List<EmailTelemetry>>()
                        ),
                        Times.Never
                    );

                    telemetryDBServiceMock
                    .Verify
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertQueueTelemetryAsync
                        (
                            It.IsAny<List<QueueTelemetry>>()
                        ),
                        Times.Never
                    );

                    telemetryDBServiceMock
                    .Verify
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertSQLTelemetryAsync
                        (
                            It.IsAny<List<SQLTelemetry>>()
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

                    var apiTelemetry = new APITelemetry();
                    var durableRestTelemetry = new DurableRestTelemetry();
                    var emailTelemetry = new EmailTelemetry();
                    var queueTelemetry = new QueueTelemetry();
                    var sqlTelemetry = new SQLTelemetry();

                    var telemetryDBServiceMock = serviceProvider.GetMock<ITelemetryDBService>();

                    //API Call Back
                    telemetryDBServiceMock
                    .Setup
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertAPITelemetryAsync
                        (
                            It.IsAny<List<APITelemetry>>()
                         )
                    );

                    //DurableRest Call Back
                    telemetryDBServiceMock
                    .Setup
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertDurableRestTelemetryAsync
                        (
                            It.IsAny<List<DurableRestTelemetry>>()
                         )
                    );

                    //Email Call Back
                    telemetryDBServiceMock
                    .Setup
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertEmailTelemetryAsync
                        (
                            It.IsAny<List<EmailTelemetry>>()
                         )
                    );

                    //Queue Call Back
                    telemetryDBServiceMock
                    .Setup
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertQueueTelemetryAsync
                        (
                            It.IsAny<List<QueueTelemetry>>()
                         )
                    );

                    //SQL Call Back
                    telemetryDBServiceMock
                    .Setup
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertSQLTelemetryAsync
                        (
                            It.IsAny<List<SQLTelemetry>>()
                         )
                    );

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    uutConcrete._queueAPITelemetry.Enqueue(apiTelemetry);
                    uutConcrete._queueDurableRestTelemetry.Enqueue(durableRestTelemetry);
                    uutConcrete._queueEmailTelemetry.Enqueue(emailTelemetry);
                    uutConcrete._queueQueueTelemetry.Enqueue(queueTelemetry);
                    uutConcrete._queueSQLTelemetry.Enqueue(sqlTelemetry);

                    //Act
                    var task = uutConcrete.Uploader(token);

                    await Task.Delay(5000).ConfigureAwait(false);
                    cancellationTokenSource.Cancel();
                    await task.ConfigureAwait(false);

                    //Assert
                    Assert.AreEqual(0, uutConcrete._queueQueueTelemetry.Count);
                    Assert.AreEqual(0, uutConcrete._queueDurableRestTelemetry.Count);
                    Assert.AreEqual(0, uutConcrete._queueEmailTelemetry.Count);
                    Assert.AreEqual(0, uutConcrete._queueQueueTelemetry.Count);
                    Assert.AreEqual(0, uutConcrete._queueSQLTelemetry.Count);

                    telemetryDBServiceMock
                    .Verify
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertAPITelemetryAsync
                        (
                            It.IsAny<List<APITelemetry>>()
                        ),
                        Times.AtLeastOnce
                    );

                    telemetryDBServiceMock
                    .Verify
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertDurableRestTelemetryAsync
                        (
                            It.IsAny<List<DurableRestTelemetry>>()
                        ),
                        Times.AtLeastOnce
                    );

                    telemetryDBServiceMock
                    .Verify
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertEmailTelemetryAsync
                        (
                            It.IsAny<List<EmailTelemetry>>()
                        ),
                        Times.AtLeastOnce
                    );

                    telemetryDBServiceMock
                    .Verify
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertQueueTelemetryAsync
                        (
                            It.IsAny<List<QueueTelemetry>>()
                        ),
                        Times.AtLeastOnce
                    );

                    telemetryDBServiceMock
                    .Verify
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertSQLTelemetryAsync
                        (
                            It.IsAny<List<SQLTelemetry>>()
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

                    var apiTelemetry = new APITelemetry();
                    var durableRestTelemetry = new DurableRestTelemetry();
                    var emailTelemetry = new EmailTelemetry();
                    var queueTelemetry = new QueueTelemetry();
                    var sqlTelemetry = new SQLTelemetry();

                    var telemetryDBServiceMock = serviceProvider.GetMock<ITelemetryDBService>();

                    //API Call Back
                    telemetryDBServiceMock
                    .Setup
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertAPITelemetryAsync
                        (
                            It.IsAny<List<APITelemetry>>()
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

                    uutConcrete._queueAPITelemetry.Enqueue(apiTelemetry);

                    //Act
                    var task = uutConcrete.Uploader(token);
                    await Task.Delay(5000).ConfigureAwait(false);
                    cancellationTokenSource.Cancel();
                    await task.ConfigureAwait(false);

                    //Assert

                    telemetryDBServiceMock
                    .Verify
                    (
                        telemetryDBServiceMock => telemetryDBServiceMock.BulkInsertAPITelemetryAsync
                        (
                            It.IsAny<List<APITelemetry>>()
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
                    var applicationLifetimeMock = serviceProvider.GetMock<IApplicationLifetime>();

                    applicationLifetimeMock
                        .SetupGet(applicationLifetime => applicationLifetime.ApplicationStopping)
                        .Returns(token);

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    cancellationTokenSource.Cancel();
                    await uutConcrete.Flush().ConfigureAwait(false);

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
                    var applicationLifetimeMock = serviceProvider.GetMock<IApplicationLifetime>();

                    applicationLifetimeMock
                        .SetupGet(applicationLifetime => applicationLifetime.ApplicationStopping)
                        .Returns(token);

                    var uut = serviceProvider.GetRequiredService<ITelemetryService>();
                    var uutConcrete = (TelemetryService)uut;

                    //Act
                    await uutConcrete.Flush().ConfigureAwait(false);

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
                RecordAPI = true,
                RecordDurableRest = true,
                RecordEmail = true,
                RecordQueue = true,
                RecordSQL = true,
                Source = "Telemetry Unit Tests"
            };

            var options = Options.Create(telemetryDBOptions);
            serviceCollection.AddSingleton<IOptions<TelemetryServiceOptions>>(options);
            serviceCollection.AddSingleton(Mock.Of<IApplicationLifetime>());
            serviceCollection.AddSingleton(Mock.Of<ITelemetryDBService>());
            serviceCollection.AddSingleton(Mock.Of<ILoggingService<TelemetryService>>());

            return serviceCollection;
        }

    }
}

using DickinsonBros.Telemetry.Abstractions.Models;
using DickinsonBros.Telemetry.Models;
using DickinsonBros.Telemetry.Services.SQL;
using DickinsonBros.Telemetry.Services.TelemetryDB;
using DickinsonBros.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DickinsonBros.Telemetry.Tests.Services.TelemetryDB
{
    [TestClass]
    public class TelemetryDBServiceTests : BaseTest
    {
        [TestMethod]
        public void GenerateDataTableAPITelemtry_Runs_ReturnsDatatableWithExpectedColumns()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<ITelemetryDBService>();
                    var uutConcrete = (TelemetryDBService)uut;

                    //Act
                    var observed = uutConcrete.GenerateDataTableAPITelemtry();

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.IsTrue(observed.Rows.Count == 0);
                    Assert.IsTrue(observed.Columns.Count == 7);
                    Assert.IsTrue(observed.Columns.Contains("CorrelationId"));
                    Assert.IsTrue(observed.Columns.Contains("ElapsedMilliseconds"));
                    Assert.IsTrue(observed.Columns.Contains("RequestRedacted"));
                    Assert.IsTrue(observed.Columns.Contains("ResponseRedacted"));
                    Assert.IsTrue(observed.Columns.Contains("Source"));
                    Assert.IsTrue(observed.Columns.Contains("StatusCode"));
                    Assert.IsTrue(observed.Columns.Contains("Url"));
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void DataTableAPITelemetry_AfterInit_ReturnsDatatableWithExpectedColumns()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<ITelemetryDBService>();
                    var uutConcrete = (TelemetryDBService)uut;

                    //Act
                    var observed = uutConcrete._dataTableAPITelemetry;

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.IsTrue(observed.Rows.Count == 0);
                    Assert.IsTrue(observed.Columns.Count == 7);
                    Assert.IsTrue(observed.Columns.Contains("CorrelationId"));
                    Assert.IsTrue(observed.Columns.Contains("ElapsedMilliseconds"));
                    Assert.IsTrue(observed.Columns.Contains("RequestRedacted"));
                    Assert.IsTrue(observed.Columns.Contains("ResponseRedacted"));
                    Assert.IsTrue(observed.Columns.Contains("Source"));
                    Assert.IsTrue(observed.Columns.Contains("StatusCode"));
                    Assert.IsTrue(observed.Columns.Contains("Url"));
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }
        
        [TestMethod]
        public async Task BulkInsertAPITelemetryAsync_Runs_CallsBulkCopyAsnycWithExpectedWithExpectedDatatable()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var telemetry = new List<APITelemetry>()
                    {
                        new APITelemetry
                        {
                            CorrelationId = "CorrelationId",
                            ElapsedMilliseconds = 1,
                            RequestRedacted = "{}",
                            ResponseRedacted = "{}",
                            Source = "",
                            StatusCode = 200,
                            Url = "www"
                        }
                    };

                    var uut = serviceProvider.GetRequiredService<ITelemetryDBService>();
                    var uutConcrete = (TelemetryDBService)uut;

                    var connectionStringObserved = (string)null;
                    var tableObserved = (DataTable)null;
                    var tableNameObserved = (string)null;
                    var batchSizeObserved = (int?)null;
                    var timeoutObserved = (TimeSpan?)null;
                    var tokenObserved = (CancellationToken?)null;

                    var telemetrySQLServiceMock = serviceProvider.GetMock<ITelemetrySQLService>();
                    telemetrySQLServiceMock
                    .Setup
                    (
                        telemetrySQLService => telemetrySQLService.BulkCopyAsync<APITelemetry>
                        (
                            It.IsAny<string>(),
                            It.IsAny<DataTable>(),
                            It.IsAny<string>(),
                            It.IsAny<int?>(),
                            It.IsAny<TimeSpan?>(),
                            It.IsAny<CancellationToken?>()
                         )
                    )
                    .Callback<string, DataTable, string, int?, TimeSpan?, CancellationToken?> ((connectionString, table, tableName, batchSize, timeout, token) =>
                    {
                        connectionStringObserved = connectionString;
                        tableObserved = table;
                        tableNameObserved = tableName;
                        batchSizeObserved = batchSize;
                        timeoutObserved = timeout;
                        tokenObserved = token;
                    });

                    //Act
                    await uutConcrete.BulkInsertAPITelemetryAsync(telemetry);


                    //Assert
                    Assert.AreEqual("ConnectionString", uutConcrete._connectionString);
                    Assert.AreEqual(TelemetryDBService.API_TELEMTRY_TABLE_NAME, tableNameObserved);
                    Assert.IsNull(batchSizeObserved);
                    Assert.AreEqual(uutConcrete._timeout, timeoutObserved);
                    Assert.IsTrue(tableObserved.Columns.Count == 7);
                    Assert.IsTrue(tableObserved.Columns.Contains("CorrelationId"));
                    Assert.IsTrue(tableObserved.Columns.Contains("ElapsedMilliseconds"));
                    Assert.IsTrue(tableObserved.Columns.Contains("RequestRedacted"));
                    Assert.IsTrue(tableObserved.Columns.Contains("ResponseRedacted"));
                    Assert.IsTrue(tableObserved.Columns.Contains("Source"));
                    Assert.IsTrue(tableObserved.Columns.Contains("StatusCode"));
                    Assert.IsTrue(tableObserved.Columns.Contains("Url"));
                    Assert.IsTrue(tableObserved.Rows.Count == 1);
                    Assert.AreEqual(telemetry.First().CorrelationId, tableObserved.Rows[0].Field<string>("CorrelationId"));
                    Assert.AreEqual(telemetry.First().ElapsedMilliseconds, tableObserved.Rows[0].Field<int>("ElapsedMilliseconds"));
                    Assert.AreEqual(telemetry.First().RequestRedacted, tableObserved.Rows[0].Field<string>("RequestRedacted"));
                    Assert.AreEqual(telemetry.First().ResponseRedacted, tableObserved.Rows[0].Field<string>("ResponseRedacted"));
                    Assert.AreEqual(telemetry.First().Source, tableObserved.Rows[0].Field<string>("Source"));
                    Assert.AreEqual(telemetry.First().StatusCode, tableObserved.Rows[0].Field<int>("StatusCode"));
                    Assert.AreEqual(telemetry.First().Url, tableObserved.Rows[0].Field<string>("Url"));

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }


        [TestMethod]
        public void GenerateDataTableDurableRestTelemtry_Runs_ReturnsDatatableWithExpectedColumns()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<ITelemetryDBService>();
                    var uutConcrete = (TelemetryDBService)uut;

                    //Act
                    var observed = uutConcrete.GenerateDataTableDurableRestTelemtry();

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.IsTrue(observed.Rows.Count == 0);
                    Assert.IsTrue(observed.Columns.Count == 10);
                    Assert.IsTrue(observed.Columns.Contains("Attempt"));
                    Assert.IsTrue(observed.Columns.Contains("BaseUrl"));
                    Assert.IsTrue(observed.Columns.Contains("CorrelationId"));
                    Assert.IsTrue(observed.Columns.Contains("ElapsedMilliseconds"));
                    Assert.IsTrue(observed.Columns.Contains("Name"));
                    Assert.IsTrue(observed.Columns.Contains("RequestRedacted"));
                    Assert.IsTrue(observed.Columns.Contains("Resource"));
                    Assert.IsTrue(observed.Columns.Contains("ResponseRedacted"));
                    Assert.IsTrue(observed.Columns.Contains("Source"));
                    Assert.IsTrue(observed.Columns.Contains("StatusCode"));
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void DataTableDurableRestTelemetry_AfterInit_ReturnsDatatableWithExpectedColumns()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<ITelemetryDBService>();
                    var uutConcrete = (TelemetryDBService)uut;

                    //Act
                    var observed = uutConcrete._dataTableDurableRestTelemetry;

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.IsTrue(observed.Rows.Count == 0);
                    Assert.IsTrue(observed.Columns.Count == 10);
                    Assert.IsTrue(observed.Columns.Contains("Attempt"));
                    Assert.IsTrue(observed.Columns.Contains("BaseUrl"));
                    Assert.IsTrue(observed.Columns.Contains("CorrelationId"));
                    Assert.IsTrue(observed.Columns.Contains("ElapsedMilliseconds"));
                    Assert.IsTrue(observed.Columns.Contains("Name"));
                    Assert.IsTrue(observed.Columns.Contains("RequestRedacted"));
                    Assert.IsTrue(observed.Columns.Contains("Resource"));
                    Assert.IsTrue(observed.Columns.Contains("ResponseRedacted"));
                    Assert.IsTrue(observed.Columns.Contains("Source"));
                    Assert.IsTrue(observed.Columns.Contains("StatusCode"));
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task BulkInsertDurableRestTelemetryAsync_Runs_CallsBulkCopyAsnycWithExpectedWithExpectedDatatable()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var telemetry = new List<DurableRestTelemetry>()
                    {
                        new DurableRestTelemetry
                        {
                            Attempt = 1,
                            BaseUrl = "www.test.com",
                            CorrelationId = "CorrelationId",
                            ElapsedMilliseconds = 1,
                            Name = "AppName",
                            RequestRedacted = "{}",
                            Resource = "/api1",
                            ResponseRedacted = "{}",
                            Source = "",
                            StatusCode = 200
                        }
                    };

                    var uut = serviceProvider.GetRequiredService<ITelemetryDBService>();
                    var uutConcrete = (TelemetryDBService)uut;

                    var connectionStringObserved = (string)null;
                    var tableObserved = (DataTable)null;
                    var tableNameObserved = (string)null;
                    var batchSizeObserved = (int?)null;
                    var timeoutObserved = (TimeSpan?)null;
                    var tokenObserved = (CancellationToken?)null;

                    var telemetrySQLServiceMock = serviceProvider.GetMock<ITelemetrySQLService>();
                    telemetrySQLServiceMock
                    .Setup
                    (
                        telemetrySQLService => telemetrySQLService.BulkCopyAsync<APITelemetry>
                        (
                            It.IsAny<string>(),
                            It.IsAny<DataTable>(),
                            It.IsAny<string>(),
                            It.IsAny<int?>(),
                            It.IsAny<TimeSpan?>(),
                            It.IsAny<CancellationToken?>()
                         )
                    )
                    .Callback<string, DataTable, string, int?, TimeSpan?, CancellationToken?>((connectionString, table, tableName, batchSize, timeout, token) =>
                    {
                        connectionStringObserved = connectionString;
                        tableObserved = table;
                        tableNameObserved = tableName;
                        batchSizeObserved = batchSize;
                        timeoutObserved = timeout;
                        tokenObserved = token;
                    });

                    //Act
                    await uutConcrete.BulkInsertDurableRestTelemetryAsync(telemetry);

                    //Assert
                    Assert.AreEqual("ConnectionString", uutConcrete._connectionString);
                    Assert.AreEqual(TelemetryDBService.DURABLEREST_TELEMTRY_TABLE_NAME, tableNameObserved);
                    Assert.IsNull(batchSizeObserved);
                    Assert.AreEqual(uutConcrete._timeout, timeoutObserved);
                    Assert.IsTrue(tableObserved.Columns.Count == 10);
                    Assert.IsTrue(tableObserved.Columns.Contains("Attempt"));
                    Assert.IsTrue(tableObserved.Columns.Contains("BaseUrl"));
                    Assert.IsTrue(tableObserved.Columns.Contains("CorrelationId"));
                    Assert.IsTrue(tableObserved.Columns.Contains("ElapsedMilliseconds"));
                    Assert.IsTrue(tableObserved.Columns.Contains("Name"));
                    Assert.IsTrue(tableObserved.Columns.Contains("RequestRedacted"));
                    Assert.IsTrue(tableObserved.Columns.Contains("Resource"));
                    Assert.IsTrue(tableObserved.Columns.Contains("ResponseRedacted"));
                    Assert.IsTrue(tableObserved.Columns.Contains("Source"));
                    Assert.IsTrue(tableObserved.Columns.Contains("StatusCode"));
                    Assert.IsTrue(tableObserved.Rows.Count == 1);
                    Assert.AreEqual(telemetry.First().Attempt, tableObserved.Rows[0].Field<int>("Attempt"));
                    Assert.AreEqual(telemetry.First().BaseUrl, tableObserved.Rows[0].Field<string>("BaseUrl"));
                    Assert.AreEqual(telemetry.First().CorrelationId, tableObserved.Rows[0].Field<string>("CorrelationId"));
                    Assert.AreEqual(telemetry.First().ElapsedMilliseconds, tableObserved.Rows[0].Field<int>("ElapsedMilliseconds"));
                    Assert.AreEqual(telemetry.First().Name, tableObserved.Rows[0].Field<string>("Name"));
                    Assert.AreEqual(telemetry.First().RequestRedacted, tableObserved.Rows[0].Field<string>("RequestRedacted"));
                    Assert.AreEqual(telemetry.First().Resource, tableObserved.Rows[0].Field<string>("Resource"));
                    Assert.AreEqual(telemetry.First().ResponseRedacted, tableObserved.Rows[0].Field<string>("ResponseRedacted"));
                    Assert.AreEqual(telemetry.First().Source, tableObserved.Rows[0].Field<string>("Source"));
                    Assert.AreEqual(telemetry.First().StatusCode, tableObserved.Rows[0].Field<int>("StatusCode"));

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }


        [TestMethod]
        public void DataTableSQLTelemetry_Runs_ReturnsDatatableWithExpectedColumns()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<ITelemetryDBService>();
                    var uutConcrete = (TelemetryDBService)uut;

                    //Act
                    var observed = uutConcrete._dataTableSQLTelemetry;

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.IsTrue(observed.Rows.Count == 0);
                    Assert.IsTrue(observed.Columns.Count == 8);
                    Assert.IsTrue(observed.Columns.Contains("CorrelationId"));
                    Assert.IsTrue(observed.Columns.Contains("Database"));
                    Assert.IsTrue(observed.Columns.Contains("ElapsedMilliseconds"));
                    Assert.IsTrue(observed.Columns.Contains("IsSuccessful"));
                    Assert.IsTrue(observed.Columns.Contains("Query"));
                    Assert.IsTrue(observed.Columns.Contains("RequestRedacted"));
                    Assert.IsTrue(observed.Columns.Contains("ResponseRedacted"));
                    Assert.IsTrue(observed.Columns.Contains("Source"));
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void GenerateDataTableSQLTelemtry_AfterInit_ReturnsDatatableWithExpectedColumns()
        {
            RunDependencyInjectedTest
           (
               (serviceProvider) =>
               {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<ITelemetryDBService>();
                   var uutConcrete = (TelemetryDBService)uut;

                    //Act
                    var observed = uutConcrete.GenerateDataTableSQLTelemtry();

                    //Assert
                    Assert.IsNotNull(observed);
                   Assert.IsTrue(observed.Rows.Count == 0);
                   Assert.IsTrue(observed.Columns.Count == 8);
                   Assert.IsTrue(observed.Columns.Contains("CorrelationId"));
                   Assert.IsTrue(observed.Columns.Contains("Database"));
                   Assert.IsTrue(observed.Columns.Contains("ElapsedMilliseconds"));
                   Assert.IsTrue(observed.Columns.Contains("IsSuccessful"));
                   Assert.IsTrue(observed.Columns.Contains("Query"));
                   Assert.IsTrue(observed.Columns.Contains("RequestRedacted"));
                   Assert.IsTrue(observed.Columns.Contains("ResponseRedacted"));
                   Assert.IsTrue(observed.Columns.Contains("Source"));
               },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task BulkInsertSQLTelemetryAsync_Runs_CallsBulkCopyAsnycWithExpectedWithExpectedDatatable()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var telemetry = new List<SQLTelemetry>()
                    {
                        new SQLTelemetry
                        {
                            CorrelationId = "CorrelationId",
                            Database = "DB",
                            ElapsedMilliseconds = 1,
                            IsSuccessful = true,
                            Query = "select * from ...",
                            RequestRedacted = "{}",
                            ResponseRedacted = "{}",
                            Source = ""
                        }
                    };

                    var uut = serviceProvider.GetRequiredService<ITelemetryDBService>();
                    var uutConcrete = (TelemetryDBService)uut;

                    var connectionStringObserved = (string)null;
                    var tableObserved = (DataTable)null;
                    var tableNameObserved = (string)null;
                    var batchSizeObserved = (int?)null;
                    var timeoutObserved = (TimeSpan?)null;
                    var tokenObserved = (CancellationToken?)null;

                    var telemetrySQLServiceMock = serviceProvider.GetMock<ITelemetrySQLService>();
                    telemetrySQLServiceMock
                    .Setup
                    (
                        telemetrySQLService => telemetrySQLService.BulkCopyAsync<APITelemetry>
                        (
                            It.IsAny<string>(),
                            It.IsAny<DataTable>(),
                            It.IsAny<string>(),
                            It.IsAny<int?>(),
                            It.IsAny<TimeSpan?>(),
                            It.IsAny<CancellationToken?>()
                         )
                    )
                    .Callback<string, DataTable, string, int?, TimeSpan?, CancellationToken?>((connectionString, table, tableName, batchSize, timeout, token) =>
                    {
                        connectionStringObserved = connectionString;
                        tableObserved = table;
                        tableNameObserved = tableName;
                        batchSizeObserved = batchSize;
                        timeoutObserved = timeout;
                        tokenObserved = token;
                    });

                    //Act
                    await uutConcrete.BulkInsertSQLTelemetryAsync(telemetry);

                    //Assert
                    Assert.AreEqual("ConnectionString", uutConcrete._connectionString);
                    Assert.AreEqual(TelemetryDBService.SQL_TELEMTRY_TABLE_NAME, tableNameObserved);
                    Assert.IsNull(batchSizeObserved);
                    Assert.AreEqual(uutConcrete._timeout, timeoutObserved);
                    Assert.IsTrue(tableObserved.Columns.Count == 8);
                    Assert.IsTrue(tableObserved.Columns.Contains("CorrelationId"));
                    Assert.IsTrue(tableObserved.Columns.Contains("Database"));
                    Assert.IsTrue(tableObserved.Columns.Contains("ElapsedMilliseconds"));
                    Assert.IsTrue(tableObserved.Columns.Contains("IsSuccessful"));
                    Assert.IsTrue(tableObserved.Columns.Contains("Query"));
                    Assert.IsTrue(tableObserved.Columns.Contains("RequestRedacted"));
                    Assert.IsTrue(tableObserved.Columns.Contains("ResponseRedacted"));
                    Assert.IsTrue(tableObserved.Columns.Contains("Source"));
                    Assert.IsTrue(tableObserved.Rows.Count == 1);
                    Assert.AreEqual(telemetry.First().CorrelationId, tableObserved.Rows[0].Field<string>("CorrelationId"));
                    Assert.AreEqual(telemetry.First().Database, tableObserved.Rows[0].Field<string>("Database"));
                    Assert.AreEqual(telemetry.First().ElapsedMilliseconds, tableObserved.Rows[0].Field<int>("ElapsedMilliseconds"));
                    Assert.AreEqual(telemetry.First().IsSuccessful, tableObserved.Rows[0].Field<bool>("IsSuccessful"));
                    Assert.AreEqual(telemetry.First().Query, tableObserved.Rows[0].Field<string>("Query"));
                    Assert.AreEqual(telemetry.First().RequestRedacted, tableObserved.Rows[0].Field<string>("RequestRedacted"));
                    Assert.AreEqual(telemetry.First().ResponseRedacted, tableObserved.Rows[0].Field<string>("ResponseRedacted"));
                    Assert.AreEqual(telemetry.First().Source, tableObserved.Rows[0].Field<string>("Source"));

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void DataTableEmailTelemetry_Runs_ReturnsDatatableWithExpectedColumns()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<ITelemetryDBService>();
                    var uutConcrete = (TelemetryDBService)uut;

                    //Act
                    var observed = uutConcrete._dataTableEmailTelemetry;

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.IsTrue(observed.Rows.Count == 0);
                    Assert.IsTrue(observed.Columns.Count == 6);
                    Assert.IsTrue(observed.Columns.Contains("CorrelationId"));
                    Assert.IsTrue(observed.Columns.Contains("ElapsedMilliseconds"));
                    Assert.IsTrue(observed.Columns.Contains("IsSuccessful"));
                    Assert.IsTrue(observed.Columns.Contains("Source"));
                    Assert.IsTrue(observed.Columns.Contains("Subject"));
                    Assert.IsTrue(observed.Columns.Contains("To"));
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void GenerateDataTableEmailTelemtry_AfterInit_ReturnsDatatableWithExpectedColumns()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<ITelemetryDBService>();
                    var uutConcrete = (TelemetryDBService)uut;

                    //Act
                    var observed = uutConcrete.GenerateDataTableEmailTelemtry();

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.IsTrue(observed.Rows.Count == 0);
                    Assert.IsTrue(observed.Columns.Count == 6);
                    Assert.IsTrue(observed.Columns.Contains("CorrelationId"));
                    Assert.IsTrue(observed.Columns.Contains("ElapsedMilliseconds"));
                    Assert.IsTrue(observed.Columns.Contains("IsSuccessful"));
                    Assert.IsTrue(observed.Columns.Contains("Source"));
                    Assert.IsTrue(observed.Columns.Contains("Subject"));
                    Assert.IsTrue(observed.Columns.Contains("To"));
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task BulkInsertEmailTelemetryAsync_Runs_CallsBulkCopyAsnycWithExpectedWithExpectedDatatable()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var telemetry = new List<EmailTelemetry>()
                    {
                        new EmailTelemetry
                        {
                            CorrelationId = "CorrelationId",
                            To = "dummyemail@dummy.com",
                            IsSuccessful = true,
                            Subject = "Welcome!",
                            ElapsedMilliseconds = 1000,
                            Source = "Test API",
                        }
                    };

                    var uut = serviceProvider.GetRequiredService<ITelemetryDBService>();
                    var uutConcrete = (TelemetryDBService)uut;

                    var connectionStringObserved = (string)null;
                    var tableObserved = (DataTable)null;
                    var tableNameObserved = (string)null;
                    var batchSizeObserved = (int?)null;
                    var timeoutObserved = (TimeSpan?)null;
                    var tokenObserved = (CancellationToken?)null;

                    var telemetrySQLServiceMock = serviceProvider.GetMock<ITelemetrySQLService>();
                    telemetrySQLServiceMock
                    .Setup
                    (
                        telemetrySQLService => telemetrySQLService.BulkCopyAsync<APITelemetry>
                        (
                            It.IsAny<string>(),
                            It.IsAny<DataTable>(),
                            It.IsAny<string>(),
                            It.IsAny<int?>(),
                            It.IsAny<TimeSpan?>(),
                            It.IsAny<CancellationToken?>()
                         )
                    )
                    .Callback<string, DataTable, string, int?, TimeSpan?, CancellationToken?>((connectionString, table, tableName, batchSize, timeout, token) =>
                    {
                        connectionStringObserved = connectionString;
                        tableObserved = table;
                        tableNameObserved = tableName;
                        batchSizeObserved = batchSize;
                        timeoutObserved = timeout;
                        tokenObserved = token;
                    });

                    //Act
                    await uutConcrete.BulkInsertEmailTelemetryAsync(telemetry);

                    //Assert
                    Assert.AreEqual("ConnectionString", uutConcrete._connectionString);
                    Assert.AreEqual(TelemetryDBService.EMAIL_TELEMTRY_TABLE_NAME, tableNameObserved);
                    Assert.IsNull(batchSizeObserved);
                    Assert.AreEqual(uutConcrete._timeout, timeoutObserved);
                    Assert.IsTrue(tableObserved.Columns.Count == 6);
                    Assert.IsTrue(tableObserved.Columns.Contains("CorrelationId"));
                    Assert.IsTrue(tableObserved.Columns.Contains("ElapsedMilliseconds"));
                    Assert.IsTrue(tableObserved.Columns.Contains("IsSuccessful"));
                    Assert.IsTrue(tableObserved.Columns.Contains("Source"));
                    Assert.IsTrue(tableObserved.Columns.Contains("Subject"));
                    Assert.IsTrue(tableObserved.Columns.Contains("To"));
                    Assert.IsTrue(tableObserved.Rows.Count == 1);
                    Assert.AreEqual(telemetry.First().CorrelationId, tableObserved.Rows[0].Field<string>("CorrelationId"));
                    Assert.AreEqual(telemetry.First().ElapsedMilliseconds, tableObserved.Rows[0].Field<int>("ElapsedMilliseconds"));
                    Assert.AreEqual(telemetry.First().IsSuccessful, tableObserved.Rows[0].Field<bool>("IsSuccessful"));
                    Assert.AreEqual(telemetry.First().Source, tableObserved.Rows[0].Field<string>("Source"));
                    Assert.AreEqual(telemetry.First().Subject, tableObserved.Rows[0].Field<string>("Subject"));
                    Assert.AreEqual(telemetry.First().To, tableObserved.Rows[0].Field<string>("To"));

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }


        [TestMethod]
        public void DataTableQueueTelemetry_Runs_ReturnsDatatableWithExpectedColumns()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<ITelemetryDBService>();
                    var uutConcrete = (TelemetryDBService)uut;

                    //Act
                    var observed = uutConcrete._dataTableQueueTelemetry;

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.IsTrue(observed.Rows.Count == 0);
                    Assert.IsTrue(observed.Columns.Count == 7);
                    Assert.IsTrue(observed.Columns.Contains("CorrelationId"));
                    Assert.IsTrue(observed.Columns.Contains("ElapsedMilliseconds"));
                    Assert.IsTrue(observed.Columns.Contains("IsSuccessful"));
                    Assert.IsTrue(observed.Columns.Contains("Name"));
                    Assert.IsTrue(observed.Columns.Contains("QueueId"));
                    Assert.IsTrue(observed.Columns.Contains("Source"));
                    Assert.IsTrue(observed.Columns.Contains("State"));

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void GenerateDataTableQueueTelemtry_AfterInit_ReturnsDatatableWithExpectedColumns()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<ITelemetryDBService>();
                    var uutConcrete = (TelemetryDBService)uut;

                    //Act
                    var observed = uutConcrete.GenerateDataTableQueueTelemtry();

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.IsTrue(observed.Rows.Count == 0);
                    Assert.IsTrue(observed.Columns.Count == 7);
                    Assert.IsTrue(observed.Columns.Contains("CorrelationId"));
                    Assert.IsTrue(observed.Columns.Contains("ElapsedMilliseconds"));
                    Assert.IsTrue(observed.Columns.Contains("IsSuccessful"));
                    Assert.IsTrue(observed.Columns.Contains("Name"));
                    Assert.IsTrue(observed.Columns.Contains("QueueId"));
                    Assert.IsTrue(observed.Columns.Contains("Source"));
                    Assert.IsTrue(observed.Columns.Contains("State"));
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task BulkInsertQueueTelemetryAsync_Runs_CallsBulkCopyAsnycWithExpectedWithExpectedDatatable()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var telemetry = new List<QueueTelemetry>()
                    {
                        new QueueTelemetry
                        {
                            CorrelationId = "CorrelationId",
                            Name = "name",
                            QueueId = 1,
                            State = QueueState.Ready,
                            IsSuccessful = true,
                            ElapsedMilliseconds = 1000,
                            Source = "Test API",
                        }
                    };

                    var uut = serviceProvider.GetRequiredService<ITelemetryDBService>();
                    var uutConcrete = (TelemetryDBService)uut;

                    var connectionStringObserved = (string)null;
                    var tableObserved = (DataTable)null;
                    var tableNameObserved = (string)null;
                    var batchSizeObserved = (int?)null;
                    var timeoutObserved = (TimeSpan?)null;
                    var tokenObserved = (CancellationToken?)null;

                    var telemetrySQLServiceMock = serviceProvider.GetMock<ITelemetrySQLService>();
                    telemetrySQLServiceMock
                    .Setup
                    (
                        telemetrySQLService => telemetrySQLService.BulkCopyAsync<APITelemetry>
                        (
                            It.IsAny<string>(),
                            It.IsAny<DataTable>(),
                            It.IsAny<string>(),
                            It.IsAny<int?>(),
                            It.IsAny<TimeSpan?>(),
                            It.IsAny<CancellationToken?>()
                         )
                    )
                    .Callback<string, DataTable, string, int?, TimeSpan?, CancellationToken?>((connectionString, table, tableName, batchSize, timeout, token) =>
                    {
                        connectionStringObserved = connectionString;
                        tableObserved = table;
                        tableNameObserved = tableName;
                        batchSizeObserved = batchSize;
                        timeoutObserved = timeout;
                        tokenObserved = token;
                    });

                    //Act
                    await uutConcrete.BulkInsertQueueTelemetryAsync(telemetry);

                    //Assert
                    Assert.AreEqual("ConnectionString", uutConcrete._connectionString);
                    Assert.AreEqual(TelemetryDBService.QUEUE_TELEMTRY_TABLE_NAME, tableNameObserved);
                    Assert.IsNull(batchSizeObserved);
                    Assert.AreEqual(uutConcrete._timeout, timeoutObserved);
                    Assert.IsNull(tokenObserved);
                    Assert.IsTrue(tableObserved.Columns.Count == 7);
                    Assert.IsTrue(tableObserved.Columns.Contains("CorrelationId"));
                    Assert.IsTrue(tableObserved.Columns.Contains("Name"));
                    Assert.IsTrue(tableObserved.Columns.Contains("QueueId"));
                    Assert.IsTrue(tableObserved.Columns.Contains("State"));
                    Assert.IsTrue(tableObserved.Columns.Contains("IsSuccessful"));
                    Assert.IsTrue(tableObserved.Columns.Contains("ElapsedMilliseconds"));
                    Assert.IsTrue(tableObserved.Columns.Contains("Source"));

                    Assert.IsTrue(tableObserved.Rows.Count == 1);
                    Assert.AreEqual(telemetry.First().CorrelationId, tableObserved.Rows[0].Field<string>("CorrelationId"));
                    Assert.AreEqual(telemetry.First().Name, tableObserved.Rows[0].Field<string>("Name"));
                    Assert.AreEqual(telemetry.First().QueueId, tableObserved.Rows[0].Field<int>("QueueId"));
                    Assert.AreEqual(telemetry.First().State, tableObserved.Rows[0].Field<QueueState>("State"));
                    Assert.AreEqual(telemetry.First().IsSuccessful, tableObserved.Rows[0].Field<bool>("IsSuccessful"));
                    Assert.AreEqual(telemetry.First().ElapsedMilliseconds, tableObserved.Rows[0].Field<int>("ElapsedMilliseconds"));
                    Assert.AreEqual(telemetry.First().Source, tableObserved.Rows[0].Field<string>("Source"));

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }



        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            var telemetryServiceOptions = new TelemetryServiceOptions
            {
                ConnectionString = "ConnectionString"
            };

            var options = Options.Create(telemetryServiceOptions);
            serviceCollection.AddSingleton<ITelemetryDBService, TelemetryDBService>();
            serviceCollection.AddSingleton<IOptions<TelemetryServiceOptions>>(options);
            serviceCollection.AddSingleton(Mock.Of<ITelemetrySQLService>());
          
            return serviceCollection;
        }

    }
}
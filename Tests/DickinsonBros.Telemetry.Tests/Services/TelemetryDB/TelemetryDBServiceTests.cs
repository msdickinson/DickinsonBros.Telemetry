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
        public void GenerateDataTableTelemtry_Runs_ReturnsDatatableWithExpectedColumns()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<ITelemetryDBService>();
                    var uutConcrete = (TelemetryDBService)uut;

                    //Act
                    var observed = uutConcrete.GenerateDataTableTelemtry();

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.IsTrue(observed.Rows.Count == 0);
                    Assert.IsTrue(observed.Columns.Count == 6);
                    Assert.IsTrue(observed.Columns.Contains("Name"));
                    Assert.IsTrue(observed.Columns.Contains("ElapsedMilliseconds"));
                    Assert.IsTrue(observed.Columns.Contains("TelemetryType"));
                    Assert.IsTrue(observed.Columns.Contains("TelemetryState"));
                    Assert.IsTrue(observed.Columns.Contains("DateTime"));
                    Assert.IsTrue(observed.Columns.Contains("Source"));
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void DataTableTelemetry_AfterInit_ReturnsDatatableWithExpectedColumns()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<ITelemetryDBService>();
                    var uutConcrete = (TelemetryDBService)uut;

                    //Act
                    var observed = uutConcrete._dataTableTelemetry;

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.IsTrue(observed.Rows.Count == 0);
                    Assert.IsTrue(observed.Columns.Count == 6);
                    Assert.IsTrue(observed.Columns.Contains("Name"));
                    Assert.IsTrue(observed.Columns.Contains("ElapsedMilliseconds"));
                    Assert.IsTrue(observed.Columns.Contains("TelemetryType"));
                    Assert.IsTrue(observed.Columns.Contains("TelemetryState"));
                    Assert.IsTrue(observed.Columns.Contains("DateTime"));
                    Assert.IsTrue(observed.Columns.Contains("Source"));
                   
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }
        
        [TestMethod]
        public async Task BulkInsertTelemetryAsync_Runs_CallsBulkCopyAsnycWithExpectedWithExpectedDatatable()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var telemetry = new List<TelemetryData>()
                    {
                        new TelemetryData
                        {
                            Name = "name",
                            ElapsedMilliseconds = 1,
                            TelemetryType =  TelemetryType.API,
                            TelemetryState = TelemetryState.Successful,
                            DateTime = new DateTime(2020, 6, 3)
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
                        telemetrySQLService => telemetrySQLService.BulkCopyAsync<TelemetryData>
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
                    await uutConcrete.BulkInsertTelemetryAsync(telemetry);


                    //Assert
                    Assert.AreEqual("ConnectionString", uutConcrete._connectionString);
                    Assert.AreEqual(TelemetryDBService.TELEMTRY_TABLE_NAME, tableNameObserved);
                    Assert.IsNull(batchSizeObserved);
                    Assert.AreEqual(uutConcrete._timeout, timeoutObserved);
                    Assert.IsTrue(tableObserved.Columns.Count == 6);
                    Assert.IsTrue(tableObserved.Columns.Contains("Name"));
                    Assert.IsTrue(tableObserved.Columns.Contains("ElapsedMilliseconds"));
                    Assert.IsTrue(tableObserved.Columns.Contains("TelemetryType"));
                    Assert.IsTrue(tableObserved.Columns.Contains("TelemetryState"));
                    Assert.IsTrue(tableObserved.Columns.Contains("DateTime"));
                    Assert.IsTrue(tableObserved.Columns.Contains("Source"));
                    Assert.IsTrue(tableObserved.Rows.Count == 1);
                    Assert.AreEqual(telemetry.First().Name, tableObserved.Rows[0].Field<string>("Name"));
                    Assert.AreEqual(telemetry.First().ElapsedMilliseconds, tableObserved.Rows[0].Field<int>("ElapsedMilliseconds"));
                    Assert.AreEqual(telemetry.First().TelemetryState, tableObserved.Rows[0].Field<TelemetryState>("TelemetryState"));
                    Assert.AreEqual(telemetry.First().TelemetryType, tableObserved.Rows[0].Field<TelemetryType>("TelemetryType"));
                    Assert.AreEqual(telemetry.First().DateTime, tableObserved.Rows[0].Field<DateTime>("DateTime"));
                    Assert.AreEqual("SampleSource", tableObserved.Rows[0].Field<string>("Source"));
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            var telemetryServiceOptions = new TelemetryServiceOptions
            {
                ConnectionString = "ConnectionString",
                Source = "SampleSource"
            };

            var options = Options.Create(telemetryServiceOptions);
            serviceCollection.AddSingleton<ITelemetryDBService, TelemetryDBService>();
            serviceCollection.AddSingleton<IOptions<TelemetryServiceOptions>>(options);
            serviceCollection.AddSingleton(Mock.Of<ITelemetrySQLService>());
          
            return serviceCollection;
        }

    }
}
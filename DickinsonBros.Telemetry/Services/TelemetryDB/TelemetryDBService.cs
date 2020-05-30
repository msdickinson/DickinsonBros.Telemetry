using DickinsonBros.Telemetry.Abstractions.Models;
using DickinsonBros.Telemetry.Models;
using DickinsonBros.Telemetry.Services.SQL;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace DickinsonBros.Telemetry.Services.TelemetryDB
{
    public class TelemetryDBService : ITelemetryDBService
    {
        internal readonly string _connectionString;
        internal readonly ITelemetrySQLService _sqlService;

        internal const string API_TELEMTRY_TABLE_NAME = "APITelemetry";
        internal const string SQL_TELEMTRY_TABLE_NAME = "SQLTelemetry";
        internal const string DURABLEREST_TELEMTRY_TABLE_NAME = "DurableRestTelemetry";
        internal const string EMAIL_TELEMTRY_TABLE_NAME = "EmailTelemetry";
        internal const string QUEUE_TELEMTRY_TABLE_NAME = "QueueTelemetry";
        internal readonly TimeSpan _timeout = TimeSpan.FromSeconds(60);

        internal readonly DataTable _dataTableAPITelemetry;
        internal readonly DataTable _dataTableSQLTelemetry;
        internal readonly DataTable _dataTableDurableRestTelemetry;
        internal readonly DataTable _dataTableEmailTelemetry;
        internal readonly DataTable _dataTableQueueTelemetry;

        public TelemetryDBService
        (
            IOptions<TelemetryServiceOptions> telemetryServiceOptions,
            ITelemetrySQLService sqlService
        )
        {
            _connectionString = telemetryServiceOptions.Value.ConnectionString;
            _sqlService = sqlService;
            _dataTableAPITelemetry          = GenerateDataTableAPITelemtry();
            _dataTableSQLTelemetry          = GenerateDataTableSQLTelemtry();
            _dataTableDurableRestTelemetry  = GenerateDataTableDurableRestTelemtry();
            _dataTableEmailTelemetry        = GenerateDataTableEmailTelemtry();
            _dataTableQueueTelemetry        = GenerateDataTableQueueTelemtry();
        }

        internal DataTable GenerateDataTableAPITelemtry()
        {
            var dataTable = new DataTable();
            dataTable.Columns.AddRange
            ( 
                new DataColumn[]
                {
                    new DataColumn("CorrelationId", typeof(string)),
                    new DataColumn("ElapsedMilliseconds", typeof(int)),
                    new DataColumn("RequestRedacted", typeof(string)),
                    new DataColumn("ResponseRedacted", typeof(string)),
                    new DataColumn("Source", typeof(string)),
                    new DataColumn("StatusCode", typeof(int)),
                    new DataColumn("Url", typeof(string))
                }
            );
            return dataTable;
        }
        internal DataTable GenerateDataTableDurableRestTelemtry()
        {
            var dataTable = new DataTable();
            dataTable.Columns.AddRange
            (
                new DataColumn[]
                {
                    new DataColumn("Attempt", typeof(int)),
                    new DataColumn("BaseUrl", typeof(string)),
                    new DataColumn("CorrelationId", typeof(string)),
                    new DataColumn("ElapsedMilliseconds", typeof(int)),
                    new DataColumn("Name", typeof(string)),
                    new DataColumn("RequestRedacted", typeof(string)),
                    new DataColumn("Resource", typeof(string)),
                    new DataColumn("ResponseRedacted", typeof(string)),
                    new DataColumn("Source", typeof(string)),
                    new DataColumn("StatusCode", typeof(int))
                }
            );

            return dataTable;
        }
        internal DataTable GenerateDataTableSQLTelemtry()
        {
            var dataTable = new DataTable();
            dataTable.Columns.AddRange
            (
                new DataColumn[]
                {
                    new DataColumn("CorrelationId", typeof(string)),
                    new DataColumn("Database", typeof(string)),
                    new DataColumn("ElapsedMilliseconds", typeof(int)),
                    new DataColumn("IsSuccessful", typeof(bool)),
                    new DataColumn("Query", typeof(string)),
                    new DataColumn("RequestRedacted", typeof(string)),
                    new DataColumn("ResponseRedacted", typeof(string)),
                    new DataColumn("Source", typeof(string))
                }
            );

            return dataTable;
        }
        internal DataTable GenerateDataTableEmailTelemtry()
        {
            var dataTable = new DataTable();
            dataTable.Columns.AddRange
            (
                new DataColumn[]
                {
                    new DataColumn("CorrelationId", typeof(string)),
                    new DataColumn("ElapsedMilliseconds", typeof(int)),
                    new DataColumn("IsSuccessful", typeof(bool)),
                    new DataColumn("Source", typeof(string)),
                    new DataColumn("Subject", typeof(string)),
                    new DataColumn("To", typeof(string))
                }
            );
            return dataTable;

        }
        internal DataTable GenerateDataTableQueueTelemtry()
        {
            var dataTable = new DataTable();
            dataTable.Columns.AddRange
            (
                new DataColumn[]
                {
                    new DataColumn("CorrelationId", typeof(string)),
                    new DataColumn("ElapsedMilliseconds", typeof(int)),
                    new DataColumn("IsSuccessful", typeof(bool)),
                    new DataColumn("Name", typeof(string)),
                    new DataColumn("QueueId", typeof(int)),
                    new DataColumn("Source", typeof(string)),
                    new DataColumn("State", typeof(int))
                }
            );

            return dataTable;
        }

        public async Task BulkInsertAPITelemetryAsync(List<APITelemetry> telemetry)
        {
            var dataTable = _dataTableAPITelemetry.Clone();

            telemetry.ForEach(e =>
                dataTable.Rows.Add
                (
                    e.CorrelationId,
                    e.ElapsedMilliseconds,
                    e.RequestRedacted,
                    e.ResponseRedacted,
                    e.Source,
                    e.StatusCode,
                    e.Url
                )
            );

            await _sqlService
                  .BulkCopyAsync<APITelemetry>
                  (
                      _connectionString,
                      dataTable,
                      API_TELEMTRY_TABLE_NAME,
                      null,
                      _timeout,
                      null
                  ).ConfigureAwait(false);
        }
        public async Task BulkInsertQueueTelemetryAsync(List<QueueTelemetry> telemetry)
        {
            var dataTable = _dataTableQueueTelemetry.Clone();

            telemetry.ForEach(e =>
                dataTable.Rows.Add
                (
                    e.CorrelationId,
                    e.ElapsedMilliseconds,
                    e.IsSuccessful,
                    e.Name,
                    e.QueueId,
                    e.Source,
                    e.State
                )
            );

            await _sqlService
                  .BulkCopyAsync<APITelemetry>
                  (
                      _connectionString,
                      dataTable,
                      QUEUE_TELEMTRY_TABLE_NAME,
                      null,
                      _timeout,
                      null
                  ).ConfigureAwait(false);
        }
        public async Task BulkInsertDurableRestTelemetryAsync(List<DurableRestTelemetry> telemetry)
        {
            var dataTable = _dataTableDurableRestTelemetry.Clone();

            telemetry.ForEach(e =>
                dataTable.Rows.Add
                (
                    e.Attempt,
                    e.BaseUrl,
                    e.CorrelationId,
                    e.ElapsedMilliseconds,
                    e.Name,
                    e.RequestRedacted,
                    e.Resource,
                    e.ResponseRedacted,
                    e.Source,
                    e.StatusCode
                )
            );

            await _sqlService
                  .BulkCopyAsync<APITelemetry>
                  (
                      _connectionString,
                      dataTable,
                      DURABLEREST_TELEMTRY_TABLE_NAME,
                      null,
                      _timeout,
                      null
                  ).ConfigureAwait(false);
        }
        public async Task BulkInsertSQLTelemetryAsync(List<SQLTelemetry> telemetry)
        {
            var dataTable = _dataTableSQLTelemetry.Clone();

            telemetry.ForEach(e =>
                dataTable.Rows.Add
                (
                    e.CorrelationId,
                    e.Database,
                    e.ElapsedMilliseconds,
                    e.IsSuccessful,
                    e.Query,
                    e.RequestRedacted,
                    e.ResponseRedacted,
                    e.Source
                )
            );

            await _sqlService
                  .BulkCopyAsync<APITelemetry>
                  (
                      _connectionString,
                      dataTable,
                      SQL_TELEMTRY_TABLE_NAME,
                      null,
                      _timeout,
                      null
                  ).ConfigureAwait(false);
        }
        public async Task BulkInsertEmailTelemetryAsync(List<EmailTelemetry> telemetry)
        {
            var dataTable = _dataTableEmailTelemetry.Clone();

            telemetry.ForEach(e =>
                dataTable.Rows.Add
                (
                    e.CorrelationId,
                    e.ElapsedMilliseconds,
                    e.IsSuccessful,
                    e.Source,
                    e.Subject,
                    e.To
                )
            );

            await _sqlService
                  .BulkCopyAsync<APITelemetry>
                  (
                      _connectionString,
                      dataTable,
                      EMAIL_TELEMTRY_TABLE_NAME,
                      null,
                      _timeout,
                      null
                  ).ConfigureAwait(false);
        }
    }
}

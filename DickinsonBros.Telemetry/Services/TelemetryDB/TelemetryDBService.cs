using DickinsonBros.Telemetry.Abstractions.Models;
using DickinsonBros.Telemetry.Models;
using DickinsonBros.Telemetry.Services.SQL;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DickinsonBros.Telemetry.Services.TelemetryDB
{
    public class TelemetryDBService : ITelemetryDBService
    {
        internal readonly string _connectionString;
        internal readonly string _source;
        
        internal readonly ITelemetrySQLService _telemetrySQLService;

        internal const string TELEMTRY_TABLE_NAME = "Telemetry.Data";
        internal readonly TimeSpan _timeout = TimeSpan.FromSeconds(60);

        internal readonly DataTable _dataTableTelemetry;

        public TelemetryDBService
        (
            IOptions<TelemetryServiceOptions> telemetryServiceOptions,
            ITelemetrySQLService telemetrySQLService
        )
        {
            _connectionString = telemetryServiceOptions.Value.ConnectionString;
            _source = telemetryServiceOptions.Value.Source;
            _telemetrySQLService = telemetrySQLService;
            _dataTableTelemetry = GenerateDataTableTelemtry();
        }

        internal DataTable GenerateDataTableTelemtry()
        {
            var dataTable = new DataTable();
            dataTable.Columns.AddRange
            ( 
                new DataColumn[]
                {
                    new DataColumn("Name", typeof(string)),
                    new DataColumn("ElapsedMilliseconds", typeof(int)),
                    new DataColumn("TelemetryType", typeof(int)),
                    new DataColumn("TelemetryState", typeof(int)),
                    new DataColumn("DateTime", typeof(DateTime)),
                    new DataColumn("Source", typeof(string))
                }
            );
            return dataTable;
        }
   
        public async Task BulkInsertTelemetryAsync(List<TelemetryData> telemetry)
        {
            var dataTable = _dataTableTelemetry.Clone();

            telemetry.ForEach(e =>
                dataTable.Rows.Add
                (
                    e.Name,
                    e.ElapsedMilliseconds,
                    e.TelemetryType,
                    e.TelemetryState,
                    e.DateTime,
                    _source
                )
            );

            await _telemetrySQLService
                  .BulkCopyAsync<TelemetryData>
                  (
                      _connectionString,
                      dataTable,
                      TELEMTRY_TABLE_NAME,
                      null,
                      _timeout,
                      null
                  ).ConfigureAwait(false);
        }
     
    }
}

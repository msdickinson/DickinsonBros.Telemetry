using DickinsonBros.Logger.Abstractions;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace DickinsonBros.Telemetry.Services.SQL
{
    [ExcludeFromCodeCoverage]
    public class TelemetrySQLService : ITelemetrySQLService
    {
        internal readonly ILoggingService<TelemetrySQLService> _logger;
        internal readonly TimeSpan DefaultBulkCopyTimeout = TimeSpan.FromMinutes(5);
        internal readonly int DefaultBatchSize = 10000;

        public TelemetrySQLService(ILoggingService<TelemetrySQLService> logger)
        {
            _logger = logger;
        }

        public async Task BulkCopyAsync<T>(string connectionString, DataTable table, string tableName, int? batchSize, TimeSpan? timeout, CancellationToken? token)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }
            if (table.Rows.Count == 0)
            {
                return;
            }

            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync(token ?? CancellationToken.None).ConfigureAwait(false);

            using SqlBulkCopy bulkCopy =
                new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, null)
                {
                    DestinationTableName = tableName,
                    BulkCopyTimeout = (int)(timeout ?? DefaultBulkCopyTimeout).TotalSeconds,
                    BatchSize = batchSize ?? DefaultBatchSize
                };

            for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
            {
                DataColumn dataColumn = table.Columns[columnIndex];
                bulkCopy.ColumnMappings.Add(dataColumn.ColumnName, dataColumn.ColumnName);
            }

            await bulkCopy.WriteToServerAsync(table, token ?? CancellationToken.None).ConfigureAwait(false);
        }

    }
}

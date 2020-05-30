using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace DickinsonBros.Telemetry.Services.SQL
{
    public interface ITelemetrySQLService
    {
        Task BulkCopyAsync<T>(string connectionString, DataTable table, string tableName, int? batchSize, TimeSpan? timeout, CancellationToken? token);
    }
}
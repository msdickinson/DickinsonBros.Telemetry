using DickinsonBros.Telemetry.Abstractions.Models;
using DickinsonBros.Telemetry.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DickinsonBros.Telemetry.Services.TelemetryDB
{
    public interface ITelemetryDBService
    {
        Task BulkInsertAPITelemetryAsync(List<APITelemetry> telemetry);
        Task BulkInsertQueueTelemetryAsync(List<QueueTelemetry> telemetry);
        Task BulkInsertDurableRestTelemetryAsync(List<DurableRestTelemetry> telemetry);
        Task BulkInsertSQLTelemetryAsync(List<SQLTelemetry> telemetry);
        Task BulkInsertEmailTelemetryAsync(List<EmailTelemetry> telemetry);

    }
}
using DickinsonBros.Telemetry.Abstractions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.Telemetry.Services.TelemetryDB
{
    public interface ITelemetryDBService
    {
        Task BulkInsertTelemetryAsync(List<TelemetryData> telemetry);

    }
}
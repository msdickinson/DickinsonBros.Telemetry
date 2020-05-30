using DickinsonBros.Telemetry.Abstractions;
using DickinsonBros.Telemetry.Services.SQL;
using DickinsonBros.Telemetry.Services.TelemetryDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.Telemetry.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddTelemetryService(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ITelemetryService, TelemetryService>();
            serviceCollection.TryAddSingleton<ITelemetrySQLService, TelemetrySQLService>();
            serviceCollection.TryAddSingleton<ITelemetryDBService, TelemetryDBService>();

            return serviceCollection;
        }
    }
}

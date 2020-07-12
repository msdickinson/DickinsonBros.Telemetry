using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Telemetry.Models
{
    [ExcludeFromCodeCoverage]
    public class TelemetryServiceOptions
    {
        public string ConnectionString { get; set; }
        public string Source { get; set; }
    }
}

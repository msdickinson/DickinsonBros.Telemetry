using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Telemetry.Models
{
    [ExcludeFromCodeCoverage]
    public class TelemetryServiceOptions
    {
        public string Source { get; set; }
        public bool RecordDurableRest { get; set; }
        public bool RecordSQL { get; set; }
        public bool RecordQueue { get; set; }
        public bool RecordAPI { get; set; }
        public bool RecordEmail { get; set; }
        public string ConnectionString { get; set; }
    }
}

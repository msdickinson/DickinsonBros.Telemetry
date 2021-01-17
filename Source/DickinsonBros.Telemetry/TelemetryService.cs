using DickinsonBros.Telemetry.Abstractions;
using DickinsonBros.Telemetry.Abstractions.Models;
using DickinsonBros.Telemetry.Models;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Telemetry
{
    public class TelemetryService : ITelemetryService
    {
        internal readonly TelemetryServiceOptions _options;

        public TelemetryService
        (
            IOptions<TelemetryServiceOptions> options
        )
        {
            _options = options.Value;
        }

        public delegate void NewTelemetryEventHandler(TelemetryData telemetryData);
        event ITelemetryService.NewTelemetryEventHandler NewTelemetryEvent;
        [ExcludeFromCodeCoverage]
        event ITelemetryService.NewTelemetryEventHandler ITelemetryService.NewTelemetryEvent
        {
            add => NewTelemetryEvent += value;
            remove => NewTelemetryEvent -= value;
        }

        public void Insert(TelemetryData telemetryData)
        {
            if (telemetryData == null)
            {
                throw new NullReferenceException();
            }

            if (string.IsNullOrWhiteSpace(telemetryData.Name))
            {
                throw new ArgumentException("Value Is Expected to have at least one char", nameof(telemetryData.Name));
            }

            if (telemetryData.DateTime == DateTime.MinValue)
            {
                throw new ArgumentException("Date Expected to be set", nameof(telemetryData.DateTime));
            }

            //Remove Any Prams If Name is a URI
            telemetryData.Name = telemetryData.Name.Split("?")[0];
            telemetryData.Name = telemetryData.Name.Substring(0, Math.Min(telemetryData.Name.Length, 255));
            telemetryData.Source = _options.Source;

            RaiseNewTelemetryEvent(telemetryData);
        }

        internal void RaiseNewTelemetryEvent(TelemetryData telemetryData)
        {
            NewTelemetryEvent?.Invoke(telemetryData);
        }

    }
}

using DickinsonBros.Logger.Abstractions;
using DickinsonBros.Telemetry.Abstractions;
using DickinsonBros.Telemetry.Abstractions.Models;
using DickinsonBros.Telemetry.Models;
using DickinsonBros.Telemetry.Services.TelemetryDB;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DickinsonBros.Telemetry
{
    public class TelemetryService : ITelemetryService
    {
        internal readonly TelemetryServiceOptions _options;
        internal readonly ILoggingService<TelemetryService> _logger;
        internal readonly ITelemetryDBService _telemetryDBService;
        internal Task _uploaderTask;
        internal bool _flush;
        internal TimeSpan _bcpInterval = TimeSpan.FromSeconds(30);
        internal CancellationTokenSource _internalTokenSource = new CancellationTokenSource();
        internal readonly ConcurrentQueue<TelemetryData> _queueTelemetry = new ConcurrentQueue<TelemetryData>();

        public TelemetryService
        (
            IOptions<TelemetryServiceOptions> options,
            IHostApplicationLifetime hostApplicationLifetime,
            ITelemetryDBService telemetryDBService,
            ILoggingService<TelemetryService> logger
        )
        {
            _options = options.Value;
            _logger = logger;
            _telemetryDBService = telemetryDBService;
            _internalTokenSource = new CancellationTokenSource();
             var _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(hostApplicationLifetime.ApplicationStopped, _internalTokenSource.Token);
            _uploaderTask = Uploader(_cancellationTokenSource.Token);
            _flush = false;
            _uploaderTask.ConfigureAwait(false);
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

            _queueTelemetry.Enqueue(telemetryData);
        }

        internal async Task Uploader(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_bcpInterval, token).ContinueWith(task => { }).ConfigureAwait(false);
                    await Upload().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogErrorRedacted($"Unhandled exception in {nameof(Uploader)}", ex);
                }
            }
        }

        internal async Task Upload()
        {
            var telemetryItems = new List<TelemetryData>();
            while (_queueTelemetry.TryDequeue(out TelemetryData telemetryData))
            {
                telemetryItems.Add(telemetryData);
            }

            await _telemetryDBService.BulkInsertTelemetryAsync(telemetryItems).ConfigureAwait(false);
        }

        public async Task FlushAsync()
        {
            //Cancel Token
            _internalTokenSource.Cancel();

            //Complete Running Process
            await _uploaderTask.ConfigureAwait(false);

            //Run Once more to flush out any left.
            await Upload().ConfigureAwait(false);
        }
    }
}

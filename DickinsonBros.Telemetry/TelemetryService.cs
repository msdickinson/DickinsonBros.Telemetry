using DickinsonBros.Logger.Abstractions;
using DickinsonBros.Telemetry.Abstractions;
using DickinsonBros.Telemetry.Abstractions.Models;
using DickinsonBros.Telemetry.Models;
using DickinsonBros.Telemetry.Services.TelemetryDB;
using Microsoft.AspNetCore.Hosting;
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
        internal readonly ConcurrentQueue<SQLTelemetry> _queueSQLTelemetry = new ConcurrentQueue<SQLTelemetry>();
        internal readonly ConcurrentQueue<APITelemetry> _queueAPITelemetry = new ConcurrentQueue<APITelemetry>();
        internal readonly ConcurrentQueue<DurableRestTelemetry> _queueDurableRestTelemetry = new ConcurrentQueue<DurableRestTelemetry>();
        internal readonly ConcurrentQueue<QueueTelemetry> _queueQueueTelemetry = new ConcurrentQueue<QueueTelemetry>();
        internal readonly ConcurrentQueue<EmailTelemetry> _queueEmailTelemetry = new ConcurrentQueue<EmailTelemetry>();

        public TelemetryService
        (
            IOptions<TelemetryServiceOptions> options,
            IApplicationLifetime applicationLifetime,
            ITelemetryDBService telemetryDBService,
            ILoggingService<TelemetryService> logger
        )
        {
            _options = options.Value;
            _logger = logger;
            _telemetryDBService = telemetryDBService;

            _internalTokenSource = new CancellationTokenSource();
             var _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(applicationLifetime.ApplicationStopped, _internalTokenSource.Token);
            _uploaderTask = Uploader(_cancellationTokenSource.Token);
            _flush = false;
            _uploaderTask.ConfigureAwait(false);
        }
        public void InsertEmail(EmailTelemetry emailTelemetry)
        {
            if (_options.RecordEmail && emailTelemetry != null)
            {
                _queueEmailTelemetry.Enqueue(emailTelemetry);
            }
        }

        public void InsertSQL(SQLTelemetry sqlTelemetry)
        {
            if (_options.RecordSQL && sqlTelemetry != null)
            {
                _queueSQLTelemetry.Enqueue(sqlTelemetry);
            }
        }

        public void InsertAPI(APITelemetry apiTelemetry)
        {
            if (_options.RecordAPI && apiTelemetry != null)
            {
                _queueAPITelemetry.Enqueue(apiTelemetry);
            }
        }

        public void InsertDurableRest(DurableRestTelemetry durableRestTelemetry)
        {
            if (_options.RecordDurableRest && durableRestTelemetry != null)
            {
                _queueDurableRestTelemetry.Enqueue(durableRestTelemetry);
            }
        }

        public void InsertQueue(QueueTelemetry queueTelemetry)
        {
            if (_options.RecordQueue && queueTelemetry != null)
            {
                _queueQueueTelemetry.Enqueue(queueTelemetry);
            }
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
            //API
            var apiTelemetryItems = new List<APITelemetry>();
            while (_queueAPITelemetry.TryDequeue(out APITelemetry apiTelemetry))
            {
                apiTelemetryItems.Add(apiTelemetry);
            }

            //Queue
            var queueTelemetryItems = new List<QueueTelemetry>();
            while (_queueQueueTelemetry.TryDequeue(out QueueTelemetry queueTelemetry))
            {
                queueTelemetryItems.Add(queueTelemetry);
            }

            //DurableRest
            var durableRestTelemetryItems = new List<DurableRestTelemetry>();
            while (_queueDurableRestTelemetry.TryDequeue(out DurableRestTelemetry durableRestTelemetry))
            {
                durableRestTelemetryItems.Add(durableRestTelemetry);
            }

            //SQL
            var sqlTelemetryItems = new List<SQLTelemetry>();
            while (_queueSQLTelemetry.TryDequeue(out SQLTelemetry sqlTelemetry))
            {
                sqlTelemetryItems.Add(sqlTelemetry);
            }

            //Email
            var emailTelemetryItems = new List<EmailTelemetry>();
            while (_queueEmailTelemetry.TryDequeue(out EmailTelemetry emailTelemetry))
            {
                emailTelemetryItems.Add(emailTelemetry);
            }

            //Bulk Insert
            await Task.WhenAll
            (
                _telemetryDBService.BulkInsertAPITelemetryAsync(apiTelemetryItems),
                _telemetryDBService.BulkInsertQueueTelemetryAsync(queueTelemetryItems),
                _telemetryDBService.BulkInsertDurableRestTelemetryAsync(durableRestTelemetryItems),
                _telemetryDBService.BulkInsertSQLTelemetryAsync(sqlTelemetryItems),
                _telemetryDBService.BulkInsertEmailTelemetryAsync(emailTelemetryItems)
            ).ConfigureAwait(false);
        }

        public async Task Flush()
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

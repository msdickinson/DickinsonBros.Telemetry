﻿using DickinsonBros.Telemetry.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DickinsonBros.Telemetry.Configurators
{
    public class TelemetryServiceOptionsConfigurator : IConfigureOptions<TelemetryServiceOptions>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public TelemetryServiceOptionsConfigurator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        void IConfigureOptions<TelemetryServiceOptions>.Configure(TelemetryServiceOptions options)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var provider = scope.ServiceProvider;
                var configuration = provider.GetRequiredService<IConfiguration>();
                var telemetryServiceOptions = configuration.GetSection(nameof(TelemetryServiceOptions)).Get<TelemetryServiceOptions>();
                configuration.Bind($"{nameof(TelemetryServiceOptions)}", options);
            }
        }
    }
}

using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.Telemetry.Models;
using DickinsonBros.Telemetry.Runner.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DickinsonBros.Telemetry.Runner.Services
{
    public class TelemetryServiceOptionsOptionsConfigurator : IConfigureOptions<TelemetryServiceOptions>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public TelemetryServiceOptionsOptionsConfigurator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        void IConfigureOptions<TelemetryServiceOptions>.Configure(TelemetryServiceOptions options)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var provider = scope.ServiceProvider;
                var configuration = provider.GetRequiredService<IConfiguration>();
                var certificateEncryptionService = provider.GetRequiredService<ICertificateEncryptionService<RunnerCertificateEncryptionServiceOptions>>();
                var telemetryServiceOptions = configuration.GetSection(nameof(TelemetryServiceOptions)).Get<TelemetryServiceOptions>();
                telemetryServiceOptions.ConnectionString = certificateEncryptionService.Decrypt(telemetryServiceOptions.ConnectionString);
                configuration.Bind($"{nameof(TelemetryServiceOptions)}", options);

                options.Source = telemetryServiceOptions.Source;
                options.ConnectionString = telemetryServiceOptions.ConnectionString;
            }
        }
    }
}

using DeviceEmulator.Model.Data.Download;
using DeviceEmulator.Web;
using EasyDesk.Tools.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceEmulatorUnitTests.Mocks
{
    public class IotHubRegistryMock : IIotHubRegistryManager
    {
        public Action<Guid, EScooterTelemetryDto> TelemetryAction { get; init; } = (_, _) => { };

        public Action<Guid, EScooterReportedDto> UpdateReportedAction { get; init; } = (_, _) => { };

        public Func<IEnumerable<EScooterTwin>> TwinsFetched { get; init; } = () => new List<EScooterTwin>();

        public Task<IEnumerable<EScooterTwin>> GetAllEScooterTwins(CancellationToken cancellationToken) =>
            Task.FromResult(TwinsFetched());

        public Task SendTelemetry(Guid id, EScooterTelemetryDto telemetry, CancellationToken c)
        {
            TelemetryAction(id, telemetry);
            return Task.CompletedTask;
        }

        public Task UpdateDevice(Guid id, EScooterReportedDto reported, CancellationToken c)
        {
            UpdateReportedAction(id, reported);
            return Task.CompletedTask;
        }
    }
}

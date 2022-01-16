using DeviceEmulator.Model.Data.Download;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceEmulator.Web
{
    public interface IIotHubRegistryManager
    {
        Task<IEnumerable<EScooterTwin>> GetAllEScooterTwins(CancellationToken cancellationToken);

        Task SendTelemetry(Guid id, EScooterTelemetryDto telemetry, CancellationToken c);

        Task UpdateDevice(Guid id, EScooterReportedDto reported, CancellationToken c);
    }
}

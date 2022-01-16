using DeviceEmulator.Model.Emulation;
using DeviceEmulator.Model.Entities;
using DeviceEmulator.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceEmulator.Model.Data.Download
{
    public class EScooterApiManager
    {
        private readonly IIotHubRegistryManager _iotHubManager;

        public EScooterApiManager(IIotHubRegistryManager iotHubManager)
        {
            _iotHubManager = iotHubManager;
        }

        public async Task<IEnumerable<ScooterSettings>> FetchEScooterList(CancellationToken cancellationToken)
        {
            var twinList = await _iotHubManager.GetAllEScooterTwins(cancellationToken);
            return twinList.Select(t => t.ToEScooterSettings());
        }

        public async Task UpdateEScooter(EScooter e, CancellationToken c)
        {
            await _iotHubManager.UpdateDevice(e.Id, ConvertEScooterToReportedDto(e), c);
        }

        public async Task SendTelemetry(EScooter e, CancellationToken c)
        {
            await _iotHubManager.SendTelemetry(e.Id, ConvertEScooterToTelemetryDto(e), c);
        }

        public bool ShouldUpdateReportedProperties(EScooter prev, EScooter next)
        {
            var p = ConvertEScooterToReportedDto(prev);
            var n = ConvertEScooterToReportedDto(next);
            return p != n;
        }

        public EScooterReportedDto ConvertEScooterToReportedDto(EScooter e)
        {
            return new EScooterReportedDto(
                e.Locked,
                e.UpdateFrequency.ToString(),
                e.MaxSpeed.MetersPerSecond,
                e.Standby);
        }

        public EScooterTelemetryDto ConvertEScooterToTelemetryDto(EScooter e)
        {
            return new EScooterTelemetryDto(
                e.BatteryLevel.Base100ValueRounded,
                e.Speed.MetersPerSecond,
                e.Position.Latitude,
                e.Position.Longitude);
        }
    }
}

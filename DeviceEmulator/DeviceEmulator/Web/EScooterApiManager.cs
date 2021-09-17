using DeviceEmulator.Model.Entities;
using DeviceEmulator.Model.Values;
using DeviceEmulator.Web;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using Geolocation;
using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceEmulator.Model.Data.Download
{
    public class EScooterApiManager
    {
        private readonly IotHubRegistryManager _iotHubManager;

        public EScooterApiManager(IotHubRegistryManager iotHubManager)
        {
            _iotHubManager = iotHubManager;
        }

        public async Task<IEnumerable<EScooter>> FetchEScooterList(CancellationToken cancellationToken)
        {
            var twinList = await _iotHubManager.GetAllEScooterTwins(cancellationToken);
            return twinList.Select(ConvertTwinToEScooter);
        }

        private EScooter ConvertTwinToEScooter(EScooterTwin twin)
        {
            return new EScooter
            {
                Id = twin.DesiredDto.Id,
                BatteryLevel = Percentage.FromPercentage(twin.ReportedDto.BatteryLevel ?? 99),
                Enabled = twin.DesiredDto.Enabled ?? false,
                Locked = twin.DesiredDto.Locked ?? true,
                MaxSpeed = Speed.FromMetersPerSecond(twin.DesiredDto.MaxSpeed ?? 10),
                PowerSavingThreshold = Percentage.FromPercentage(twin.DesiredDto.PowerSavingThreshold ?? 20),
                StandbyThreshold = Percentage.FromPercentage(twin.DesiredDto.StandbyThreshold ?? 10),
                UpdateFrequency = Duration.Parse(twin.DesiredDto.UpdateFrequency ?? "0:1" /* 1 minutes*/),
                Position = new Coordinate(twin.ReportedDto.Latitude ?? 0, twin.ReportedDto.Longitude ?? 0)
            };
        }

        public async void UpdateEScooter(EScooter e, CancellationToken c)
        {
            await _iotHubManager.UpdateDevice(ConvertEScooterToReportedDto(e), c);
        }

        private EScooterReportedDto ConvertEScooterToReportedDto(EScooter e)
        {
            return new EScooterReportedDto(
                e.Id,
                e.Locked,
                e.Enabled,
                e.UpdateFrequency.ToString(),
                e.MaxSpeed.MetersPerSecond,
                (int)e.BatteryLevel.Base100Value,
                (int)e.PowerSavingThreshold.Base100Value,
                (int)e.StandbyThreshold.Base100Value,
                e.Position.Latitude,
                e.Position.Longitude);
        }
    }
}

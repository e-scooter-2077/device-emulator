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
                Id = twin.Id,
                Locked = twin.DesiredDto.Locked ?? true,
                MaxSpeed = Speed.FromMetersPerSecond(twin.DesiredDto.MaxSpeed ?? 10),
                UpdateFrequency = Duration.Parse(twin.DesiredDto.UpdateFrequency ?? "0:1" /* 1 minutes*/),
                Unsynced = twin.ShouldUpdate()
            };
        }

        public async Task UpdateEScooter(EScooter e, CancellationToken c)
        {
            await _iotHubManager.UpdateDevice(e.Id, ConvertEScooterToReportedDto(e), c);
        }

        public EScooterReportedDto ConvertEScooterToReportedDto(EScooter e)
        {
            return new EScooterReportedDto(
                e.Locked,
                e.UpdateFrequency.ToString(),
                e.MaxSpeed.MetersPerSecond,
                e.Standby);
        }
    }
}

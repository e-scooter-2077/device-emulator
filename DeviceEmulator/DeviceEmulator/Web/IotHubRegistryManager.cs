using DeviceEmulator.Model.Data.Download;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceEmulator.Web
{
    public class IotHubRegistryManager
    {
        private readonly IotHubConfiguration _iotHubConfiguration;
        private readonly RegistryManager _registryManager;

        public IotHubRegistryManager(IotHubConfiguration iotHubConfiguration)
        {
            _iotHubConfiguration = iotHubConfiguration;
            _registryManager = RegistryManager.CreateFromConnectionString(_iotHubConfiguration.ConnectionString);
        }

        public async Task<IEnumerable<EScooterTwin>> GetAllEScooterTwins(CancellationToken cancellationToken)
        {
            IQuery query = _registryManager.CreateQuery("SELECT * FROM devices WHERE tags.type = 'EScooter'");
            List<EScooterTwin> result = new();
            while (query.HasMoreResults && !cancellationToken.IsCancellationRequested)
            {
                IEnumerable<Twin> page = await query.GetNextAsTwinAsync();
                result.AddRange(page.Select(MapIoTHubTwinToEScooterTwin));
            }
            return result;
        }

        private EScooterTwin MapIoTHubTwinToEScooterTwin(Twin twin)
        {
            var desired = JsonConvert.DeserializeObject<EScooterDesiredDto>(twin.Properties.Desired.ToJson());
            var reported = JsonConvert.DeserializeObject<EScooterReportedDto>(twin.Properties.Reported.ToJson());

            return new EScooterTwin(desired, reported);
        }

        public async Task<string> GetDevicePrimaryKey(Guid deviceId)
        {
            var device = await _registryManager.GetDeviceAsync(deviceId.ToString());
            return device.Authentication.SymmetricKey.PrimaryKey;
        }

        public async Task<EScooterReportedDto> UpdateDevice(EScooterReportedDto aaa, CancellationToken c)
        {
            return aaa;
            throw new NotImplementedException();
        }
    }
}

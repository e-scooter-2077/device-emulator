using DeviceEmulator.Model.Data.Download;
using EasyDesk.Tools;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransportType = Microsoft.Azure.Devices.Client.TransportType;

namespace DeviceEmulator.Web
{
    public class IotHubRegistryManager
    {
        private readonly IotHubConfiguration _iotHubConfiguration;
        private readonly RegistryManager _registryManager;
        private readonly string _hostName;

        public IotHubRegistryManager(IotHubConfiguration iotHubConfiguration)
        {
            _iotHubConfiguration = iotHubConfiguration;
            _registryManager = RegistryManager.CreateFromConnectionString(_iotHubConfiguration.ConnectionString);
            _hostName = _iotHubConfiguration.HostName;
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

            return new EScooterTwin(Guid.Parse(twin.DeviceId), desired, reported);
        }

        public async Task<string> GetDevicePrimaryKey(Guid deviceId)
        {
            var device = await _registryManager.GetDeviceAsync(deviceId.ToString());
            return device.Authentication.SymmetricKey.PrimaryKey;
        }

        public async Task UpdateDevice(Guid id, EScooterReportedDto reported, CancellationToken c)
        {
            if (!c.IsCancellationRequested)
            {
                var authMethod = new DeviceAuthenticationWithRegistrySymmetricKey(id.ToString(), await GetDevicePrimaryKey(id));
                ClientOptions options = null;
                DeviceClient deviceClient = DeviceClient.Create(_hostName, authMethod, TransportType.Mqtt, options);

                TwinCollection reportedProperties = new TwinCollection(JsonConvert.SerializeObject(reported));
                await deviceClient.UpdateReportedPropertiesAsync(reportedProperties);
            }
        }
    }
}

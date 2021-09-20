using DeviceEmulator.Model.Data.Download;
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
            var desired = JsonConvert.DeserializeObject<EScooterDesiredDto>(twin.Properties.Desired.ToJson()); // TODO: Insert deviceID
            var reported = JsonConvert.DeserializeObject<EScooterReportedDto>(twin.Properties.Reported.ToJson()); // TODO: Insert deviceID

            return new EScooterTwin(Guid.Parse(twin.DeviceId), desired, reported);
        }

        public async Task<string> GetDevicePrimaryKey(Guid deviceId)
        {
            var device = await _registryManager.GetDeviceAsync(deviceId.ToString());
            return device.Authentication.SymmetricKey.PrimaryKey;
        }

        public async Task<Task> UpdateDevice(Guid id, EScooterReportedDto reported, CancellationToken c)
        {
            var authMethod = new DeviceAuthenticationWithRegistrySymmetricKey(id.ToString(), await GetDevicePrimaryKey(id));
            ClientOptions options = null;
            /*var options = new ClientOptions
            {
                ModelId = "dtmi:com:example:TemperatureController;2",
            };*/
            DeviceClient deviceClient = DeviceClient.Create(_hostName, authMethod, TransportType.Mqtt, options);

            TwinCollection reportedProperties = new TwinCollection(JsonConvert.SerializeObject(reported));
            return deviceClient.UpdateReportedPropertiesAsync(reportedProperties);
        }
    }
}

using DeviceEmulator.Model.Data.Download;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Exceptions;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Message = Microsoft.Azure.Devices.Client.Message;
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
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            _iotHubConfiguration = iotHubConfiguration;
            _registryManager = RegistryManager.CreateFromConnectionString(_iotHubConfiguration.ConnectionString);
            _hostName = _iotHubConfiguration.HostName;
        }

        public async Task<IEnumerable<EScooterTwin>> GetAllEScooterTwins(CancellationToken cancellationToken)
        {
            var query = _registryManager.CreateQuery("SELECT * FROM devices WHERE tags.type = 'EScooter'");
            var result = new List<EScooterTwin>();
            while (query.HasMoreResults && !cancellationToken.IsCancellationRequested)
            {
                var page = await query.GetNextAsTwinAsync();
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

        private async Task<string> GetDevicePrimaryKey(Guid deviceId, CancellationToken c)
        {
            var device = await _registryManager.GetDeviceAsync(deviceId.ToString(), c);
            return device.Authentication.SymmetricKey.PrimaryKey;
        }

        public async Task UpdateDevice(Guid id, EScooterReportedDto reported, CancellationToken c)
        {
            var deviceClient = await GetDeviceClient(id, c);
            var reportedProperties = new TwinCollection(JsonConvert.SerializeObject(reported));
            try
            {
                await deviceClient.UpdateReportedPropertiesAsync(reportedProperties, c);
            }
            catch (IotHubCommunicationException e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                await deviceClient.CloseAsync();
                deviceClient.Dispose();
            }
        }

        private async Task<DeviceClient> GetDeviceClient(Guid id, CancellationToken c, ClientOptions options = null)
        {
            var authMethod = new DeviceAuthenticationWithRegistrySymmetricKey(id.ToString(), await GetDevicePrimaryKey(id, c));
            return DeviceClient.Create(_hostName, authMethod, TransportType.Mqtt, options);
        }

        public async Task SendTelemetry(Guid id, EScooterTelemetryDto telemetry, CancellationToken c)
        {
            var deviceClient = await GetDeviceClient(id, c);
            var message = CreateMessageFromJson(JsonConvert.SerializeObject(telemetry));
            try
            {
                await deviceClient.SendEventAsync(message, c);
            }
            catch (IotHubCommunicationException e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                message.Dispose();
                await deviceClient.CloseAsync();
                deviceClient.Dispose();
            }
        }

        private Message CreateMessageFromJson(string jsonSerializedTelemetry, Encoding encoding = default)
        {
            var messageEncoding = encoding ?? Encoding.UTF8;
            return new Message(messageEncoding.GetBytes(jsonSerializedTelemetry))
            {
                ContentEncoding = messageEncoding.WebName,
                ContentType = "application/json",
            };
        }
    }
}

using DeviceEmulator.Extensions;
using DeviceEmulator.Model.Data.Download;
using DeviceEmulator.Model.Values;
using DeviceEmulator.Web;
using EasyDesk.CleanArchitecture.Domain.Time;
using EasyDesk.CleanArchitecture.Infrastructure.DependencyInjection;
using EasyDesk.CleanArchitecture.Infrastructure.Time;
using Geolocation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceEmulator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddConfigAsSingleton<IotHubConfiguration>(hostContext.Configuration);
                    services.AddSingleton<IotHubRegistryManager>();
                    services.AddSingleton<EScooterApiManager>();
                    services.AddTimestampProvider(hostContext.Configuration);
                    services.AddHostedService<Worker>();
                });
    }
}

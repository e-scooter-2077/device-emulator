using DeviceEmulator.Model.Data.Download;
using DeviceEmulator.Web;
using EasyDesk.CleanArchitecture.Infrastructure.DependencyInjection;
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
                    services.AddHostedService<Worker>();
                });
    }
}

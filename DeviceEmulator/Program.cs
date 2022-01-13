using DeviceEmulator.Model.Data.Download;
using DeviceEmulator.Web;
using EasyDesk.CleanArchitecture.Infrastructure.DependencyInjection;
using EasyDesk.CleanArchitecture.Infrastructure.Time;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace DeviceEmulator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e) when (e is Microsoft.Azure.Devices.Client.Exceptions.IotHubException || e is TaskCanceledException || e is Microsoft.Azure.Devices.Common.Exceptions.IotHubException)
            {
                Console.WriteLine("Exiting...");
                Environment.Exit(1);
            }
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

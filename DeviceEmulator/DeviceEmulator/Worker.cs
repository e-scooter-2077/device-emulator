using DeviceEmulator.Model.Data.Download;
using DeviceEmulator.Model.Emulation;
using DeviceEmulator.Model.Entities;
using DeviceEmulator.Web;
using EasyDesk.CleanArchitecture.Domain.Time;
using EasyDesk.Tools;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceEmulator
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly EScooterEmulator _emulator;

        public Worker(ILogger<Worker> logger, EScooterApiManager apiManager, ITimestampProvider timestampProvider)
        {
            _logger = logger;
            _emulator = new EScooterEmulator(timestampProvider)
            {
                EscooterListLoader = apiManager.FetchEScooterList,
                EScooterUpdatedCallback = async (EScooter e, CancellationToken c) =>
                {
                    await apiManager.UpdateEScooter(e, c);
                    Console.WriteLine("Property update sent:");
                    Console.WriteLine(e);
                    Console.WriteLine();
                    return Nothing.Value;
                },
                EScooterTelemetryCallback = async (EScooter e, CancellationToken c) =>
                {
                    // TODO: Send Telemetry
                    await apiManager.SendTelemetry(e, c);
                    Console.WriteLine("Telemetry sent:");
                    Console.WriteLine(e);
                    Console.WriteLine();
                    return Nothing.Value;
                }
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await _emulator.EmulateIteration(stoppingToken);
                await Task.Delay(20 * 1000, stoppingToken);
            }
        }
    }
}

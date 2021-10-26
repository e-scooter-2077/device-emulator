using DeviceEmulator.Model.Data.Download;
using DeviceEmulator.Model.Emulation;
using DeviceEmulator.Model.Entities;
using EasyDesk.CleanArchitecture.Domain.Time;
using EasyDesk.Tools;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceEmulator
{
    public class Worker : IHostedService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITimestampProvider _timestampProvider;
        private readonly EScooterEmulator _emulator;

        public Worker(ILogger<Worker> logger, EScooterApiManager apiManager, ITimestampProvider timestampProvider)
        {
            _logger = logger;
            _timestampProvider = timestampProvider;
            _emulator = new EScooterEmulator(_timestampProvider)
            {
                EscooterListLoader = async (token) => await apiManager.FetchEScooterList(token),
                EScooterUpdatedCallback = async (EScooter prev, EScooter next, CancellationToken c) =>
                {
                    if (apiManager.ShouldUpdateReportedProperties(prev, next))
                    {
                        await apiManager.UpdateEScooter(next, c);
                        Console.WriteLine($"[{next.Id}] Property update sent:");
                        Console.WriteLine(JsonConvert.SerializeObject(apiManager.ConvertEScooterToReportedDto(next), Formatting.Indented));
                        Console.WriteLine();
                    }
                },
                EScooterTelemetryCallback = async (EScooter prev, EScooter next, CancellationToken c) =>
                {
                    await apiManager.SendTelemetry(next, c);
                    Console.WriteLine($"[{next.Id}] Telemetry sent:");
                    Console.WriteLine(JsonConvert.SerializeObject(apiManager.ConvertEScooterToTelemetryDto(next), Formatting.Indented));
                    Console.WriteLine();
                }
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken) => await ExecuteAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        protected async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", _timestampProvider.Now);
                await _emulator.EmulateIteration(stoppingToken);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}

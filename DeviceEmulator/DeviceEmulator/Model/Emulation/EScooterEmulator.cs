using DeviceEmulator.Extensions;
using DeviceEmulator.Model.Entities;
using DeviceEmulator.Model.Values;
using DeviceEmulator.Web;
using EasyDesk.CleanArchitecture.Domain.Time;
using EasyDesk.Tools;
using EasyDesk.Tools.Collections;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using Geolocation;
using Microsoft.VisualBasic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceEmulator.Model.Emulation
{
    public class EScooterEmulator
    {
        public AsyncFunc<CancellationToken, IEnumerable<ScooterSettings>> EscooterListLoader { get; init; } = _ => Task.FromResult(Enumerable.Empty<ScooterSettings>());

        public AsyncAction<EScooter, EScooter, CancellationToken> EScooterUpdatedCallback { get; init; } = (_, _, _) => Task.FromResult(Nothing.Value);

        public AsyncAction<EScooter, EScooter, CancellationToken> EScooterTelemetryCallback { get; init; } = (_, _, _) => Task.FromResult(Nothing.Value);

        public EScooterEmulator(ITimestampProvider timestampProvider)
        {
            _timestampProvider = timestampProvider;
        }

        private readonly IDictionary<Guid, EScooterStatus> _escooterMap = new Dictionary<Guid, EScooterStatus>();
        private readonly ITimestampProvider _timestampProvider;

        public async Task EmulateIteration(CancellationToken stoppingToken)
        {
            var scooters = await EscooterListLoader(stoppingToken);
            foreach (ScooterSettings settings in scooters)
            {
                if (!_escooterMap.ContainsKey(settings.Id))
                {
                    _escooterMap[settings.Id] = new EScooterStatus(settings.ToEScooterWithDefaults(), _timestampProvider.Now, _timestampProvider.Now);
                    Console.WriteLine($"Added device: [{settings.Id}] with settings:\n{settings}");
                }
                if (settings.Unsynced)
                {
                    var prev = _escooterMap[settings.Id].Scooter;
                    _escooterMap[settings.Id].UpdateFromNewSettings(settings);
                    await EScooterUpdatedCallback(prev, _escooterMap[settings.Id].Scooter, stoppingToken);
                }
            }

            foreach (EScooterStatus scooterStatus in _escooterMap.Values)
            {
                var previous = scooterStatus.Scooter;
                scooterStatus.Update(_timestampProvider.Now);

                if (!stoppingToken.IsCancellationRequested)
                {
                    await EScooterUpdatedCallback(previous, scooterStatus.Scooter, stoppingToken);
                }
                if (!stoppingToken.IsCancellationRequested && scooterStatus.TelemetryCheck(_timestampProvider.Now))
                {
                    await EScooterTelemetryCallback(previous, scooterStatus.Scooter, stoppingToken);
                }
            }
        }
    }
}

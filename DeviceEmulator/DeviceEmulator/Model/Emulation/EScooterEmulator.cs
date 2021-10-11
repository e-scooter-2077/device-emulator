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
        private static readonly Distance _distancePerBatteryPercent = Distance.FromMeters(450);

        public AsyncFunc<CancellationToken, IEnumerable<EScooter>> EscooterListLoader { get; init; } = _ => Task.FromResult(Enumerable.Empty<EScooter>());

        public AsyncFunc<EScooter, CancellationToken, Nothing> EScooterUpdatedCallback { get; init; } = (_, _) => Task.FromResult(Nothing.Value);

        public AsyncFunc<EScooter, CancellationToken, Nothing> EScooterTelemetryCallback { get; init; } = (_, _) => Task.FromResult(Nothing.Value);

        public EScooterEmulator(ITimestampProvider timestampProvider)
        {
            _timestampProvider = timestampProvider;
        }

        private readonly IDictionary<Guid, EScooterStatus> _escooterMap = new Dictionary<Guid, EScooterStatus>();
        private readonly ITimestampProvider _timestampProvider;

        public async Task EmulateIteration(CancellationToken stoppingToken)
        {
            foreach (EScooter scooter in await EscooterListLoader(stoppingToken))
            {
                var telemetryUpdate = false;
                _escooterMap.Merge(
                    scooter.Id,
                    new EScooterStatus(scooter, _timestampProvider.Now, _timestampProvider.Now),
                    (_, prev) => ComputeEScooterUpdate(scooter, prev, out telemetryUpdate));
                var newStatus = _escooterMap[scooter.Id];
                if (!stoppingToken.IsCancellationRequested && scooter.Unsynced)
                {
                    await EScooterUpdatedCallback(newStatus.Scooter, stoppingToken);
                }
                if (!stoppingToken.IsCancellationRequested && telemetryUpdate)
                {
                    await EScooterTelemetryCallback(newStatus.Scooter, stoppingToken);
                }
            }
        }

        private EScooterStatus ComputeEScooterUpdate(EScooter iotHubScooter, EScooterStatus previous, out bool telemetryUpdate)
        {
            var durationSinceLastStatusUpdate = _timestampProvider.DurationSince(previous.LastStatusUpdate);
            var newScooter = previous.Scooter;

            if (iotHubScooter.Unsynced)
            {
                // Update new with emulated data
                newScooter = iotHubScooter with
                {
                    BatteryLevel = previous.Scooter.BatteryLevel,
                    Direction = previous.Scooter.Direction,
                    Position = previous.Scooter.Position,
                    Speed = previous.Scooter.Speed,
                    Unsynced = false
                };
            }

            var random = new Random();

            if (!newScooter.Locked && newScooter.BatteryLevel.Base100ValueRounded > 0)
            {
                if (newScooter.BatteryLevel < newScooter.StandbyThreshold)
                {
                    newScooter = newScooter with
                    {
                        Acceleration = Acceleration.FromKilometersPerHourPerSecond(-5)
                    };
                }
                else
                {
                    var randomDecision = random.NextDouble();
                    if (randomDecision < 0.1)
                    {
                        newScooter = newScooter with
                        {
                            Speed = newScooter.Speed * 0.1,
                            Acceleration = Acceleration.FromMetersPerSecondSquared(1)
                        };
                    }
                    else if (randomDecision < 0.6)
                    {
                        newScooter = newScooter with
                        {
                            Acceleration = Acceleration.FromKilometersPerHourPerSecond(1.5)
                        };
                    }
                    else
                    {
                        newScooter = newScooter with
                        {
                            Acceleration = Acceleration.FromKilometersPerHourPerSecond(random.NextDouble() - 0.5)
                        };
                    }
                }
                newScooter = newScooter with
                {
                    Speed = newScooter.Speed + newScooter.Acceleration * durationSinceLastStatusUpdate.AsTimeSpan
                };
            }
            else
            {
                newScooter = newScooter with
                {
                    Speed = random.NextDouble() > 0.7 ? 0.01 : 0 // E-Scooters tend to be moved manually
                };
            }

            newScooter = newScooter with
            {
                Speed = newScooter.Speed < newScooter.MaxSpeed ? newScooter.Speed : newScooter.MaxSpeed
            };

            if (newScooter.Speed > Speed.FromKilometersPerHour(0))
            {
                newScooter = newScooter with
                {
                    Direction = random.NextDouble() > 0.2 ? Direction.FromDegrees(newScooter.Direction.Degrees + (random.NextDouble() > 0.4 ? 10 : -11)) : newScooter.Direction.Degrees
                };
                var newPosition = MoveScooter(newScooter, durationSinceLastStatusUpdate);
                var traveledMeters = GeoCalculator.GetDistance(newScooter.Position, newPosition, decimalPlaces: 2, DistanceUnit.Meters);
                var batteryConsumedPercentage = traveledMeters / _distancePerBatteryPercent.Meters;
                newScooter = newScooter with
                {
                    Position = newPosition,
                    BatteryLevel = Fraction.FromPercentage(newScooter.BatteryLevel.Base100Value - batteryConsumedPercentage)
                };
            }

            telemetryUpdate = _timestampProvider.DurationSince(previous.LastTelemetryUpdate) >= newScooter.UpdateFrequency;
            return new EScooterStatus(
                    newScooter,
                    _timestampProvider.Now,
                    telemetryUpdate ? _timestampProvider.Now : previous.LastTelemetryUpdate);
        }

        public Coordinate MoveScooter(EScooter scooter, Duration movementDuration)
        {
            return scooter.Position.MoveBy(
                scooter.Direction,
                Distance.Min(
                    scooter.Speed * movementDuration.AsTimeSpan,
                    Distance.FromMeters(scooter.BatteryLevel.Base100Value * _distancePerBatteryPercent.Meters)));
        }
    }
}

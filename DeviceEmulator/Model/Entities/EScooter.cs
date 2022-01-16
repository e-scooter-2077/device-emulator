using DeviceEmulator.Extensions;
using DeviceEmulator.Model.Values;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using Geolocation;
using System;

namespace DeviceEmulator.Model.Entities
{
    public record EScooter(
        Guid Id,
        bool Locked,
        Fraction BatteryLevel,
        Fraction StandbyThreshold,
        Acceleration Acceleration,
        Speed Speed,
        Speed StandbyMaxSpeed,
        Speed DesiredMaxSpeed,
        Direction Direction,
        Coordinate Position,
        Distance DistancePerBatteryPercent,
        Duration UpdateFrequency)
    {

        public Speed MaxSpeed => Standby ? StandbyMaxSpeed : DesiredMaxSpeed;

        public bool Standby => BatteryLevel <= StandbyThreshold;
    }

    public static class EScooterExtensions
    {
        public static EScooter SimulateRandomUsage(this EScooter result, Duration duration)
        {
            var random = new Random();

            if (!result.Locked && result.BatteryLevel.Base100ValueRounded > 0)
            {
                if (result.Standby)
                {
                    result = result with { Acceleration = Acceleration.FromKilometersPerHourPerSecond(-5) };
                }
                else
                {
                    var randomDecision = random.NextDouble();
                    if (randomDecision < 0.1)
                    {
                        result = result with
                        {
                            Speed = result.Speed * 0.1,
                            Acceleration = Acceleration.FromMetersPerSecondSquared(1)
                        };
                    }
                    else if (randomDecision < 0.6)
                    {
                        result = result with { Acceleration = Acceleration.FromKilometersPerHourPerSecond(1.5) };
                    }
                    else
                    {
                        result = result with { Acceleration = Acceleration.FromKilometersPerHourPerSecond(random.NextDouble() - 0.5) };
                    }
                }
                var delta = result.Acceleration * duration.AsTimeSpan;
                result = result with { Speed = result.Speed + delta };
            }
            else
            {
                result = result with { Speed = Speed.FromMetersPerSecond(random.NextDouble() > 0.7 ? 0.01 : 0) }; // E-Scooters tend to be moved manually
            }

            result = result with
            {
                Speed = result.Speed < result.MaxSpeed ? result.Speed : result.MaxSpeed
            };

            if (result.Speed > Speed.FromKilometersPerHour(0))
            {
                result = result with
                {
                    Direction = Direction.FromDegrees(random.NextDouble() > 0.2 ? result.Direction.Degrees + (random.NextDouble() > 0.4 ? 10 : -11) : result.Direction.Degrees)
                };

                var oldPosition = result.Position;
                result = result.MoveFor(duration);
                var traveledMeters = GeoCalculator.GetDistance(oldPosition, result.Position, decimalPlaces: 2, DistanceUnit.Meters);
                var batteryConsumedPercentage = traveledMeters / result.DistancePerBatteryPercent.Meters;
                result = result with
                {
                    BatteryLevel = Fraction.FromPercentage(result.BatteryLevel.Base100Value - batteryConsumedPercentage)
                };
            }
            return result;
        }

        private static EScooter MoveFor(this EScooter result, Duration movementDuration)
        {
            return result with
            {
                Position = result.Position.MoveBy(
                    result.Direction,
                    Distance.Min(
                        result.Speed * movementDuration.AsTimeSpan,
                        Distance.FromMeters(result.BatteryLevel.Base100Value * result.DistancePerBatteryPercent.Meters)))
            };
        }
    }
}

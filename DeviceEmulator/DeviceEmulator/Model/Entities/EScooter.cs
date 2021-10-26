using DeviceEmulator.Extensions;
using DeviceEmulator.Model.Values;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using Geolocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public EScooter SimulateRandomUsage(Duration duration)
        {
            var random = new Random();
            var result = this;

            if (!Locked && BatteryLevel.Base100ValueRounded > 0)
            {
                if (Standby)
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
                            Speed = Speed * 0.1,
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
                var delta = Acceleration * duration.AsTimeSpan;
                result = result with { Speed = Speed + delta };
            }
            else
            {
                result = result with { Speed = Speed.FromMetersPerSecond(random.NextDouble() > 0.7 ? 0.01 : 0) }; // E-Scooters tend to be moved manually
            }

            result = result with
            {
                Speed = Speed < MaxSpeed ? Speed : MaxSpeed
            };

            if (Speed > Speed.FromKilometersPerHour(0))
            {
                result = result with
                {
                    Direction = random.NextDouble() > 0.2 ? Direction.FromDegrees(Direction.Degrees + (random.NextDouble() > 0.4 ? 10 : -11)) : Direction.Degrees
                };

                var oldPosition = Position;
                result = result.MoveFor(duration);
                var traveledMeters = GeoCalculator.GetDistance(oldPosition, Position, decimalPlaces: 2, DistanceUnit.Meters);
                var batteryConsumedPercentage = traveledMeters / DistancePerBatteryPercent.Meters;
                result = result with
                {
                    BatteryLevel = Fraction.FromPercentage(BatteryLevel.Base100Value - batteryConsumedPercentage)
                };
            }
            return result;
        }

        private EScooter MoveFor(Duration movementDuration)
        {
            return this with
            {
                Position = Position.MoveBy(
                    Direction,
                    Distance.Min(
                        Speed * movementDuration.AsTimeSpan,
                        Distance.FromMeters(BatteryLevel.Base100Value * DistancePerBatteryPercent.Meters)))
            };
        }
    }
}

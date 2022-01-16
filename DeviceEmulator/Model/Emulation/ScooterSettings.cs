using DeviceEmulator.Model.Entities;
using DeviceEmulator.Model.Values;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using Geolocation;
using System;

namespace DeviceEmulator.Model.Emulation
{
    public record ScooterSettings(
        Guid Id,
        bool Unsynced,
        bool? Locked,
        Duration UpdateFrequency,
        Speed MaxSpeed)
    {
        public EScooter ToEScooterWithDefaults()
        {
            double latitudeOffset = 0.0001 * new Random().Next(-10, 10);
            double longitudeOffset = 0.0001 * new Random().Next(-10, 10);

            return new EScooter(
                Id: Id,
                Locked: Locked ?? true,
                BatteryLevel: Fraction.FromPercentage(100), // Change here for resetting battery level
                StandbyThreshold: Fraction.FromPercentage(10),
                Acceleration: Acceleration.FromKilometersPerHourPerSecond(0),
                Speed: Speed.FromMetersPerSecond(0),
                StandbyMaxSpeed: Speed.FromKilometersPerHour(15),
                DesiredMaxSpeed: MaxSpeed ?? Speed.FromKilometersPerHour(30),
                Direction: Direction.East,
                Position: new Coordinate(44.143043 + latitudeOffset, 12.247474 + longitudeOffset), // Cesena: 44.143043, 12.247474);
                DistancePerBatteryPercent: Distance.FromMeters(450),
                UpdateFrequency: UpdateFrequency ?? Duration.FromMinutes(1));
        }
    }
}

using DeviceEmulator.Model.Entities;
using DeviceEmulator.Model.Values;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using Geolocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return new EScooter(
                Id: Id,
                Locked: Locked ?? true,
                BatteryLevel: Fraction.FromPercentage(100),
                StandbyThreshold: Fraction.FromPercentage(10),
                Acceleration: Acceleration.FromKilometersPerHourPerSecond(0),
                Speed: Speed.FromMetersPerSecond(0),
                StandbyMaxSpeed: Speed.FromKilometersPerHour(15),
                DesiredMaxSpeed: MaxSpeed ?? Speed.FromKilometersPerHour(30),
                Direction: Direction.East,
                Position: new Coordinate(44.143043, 12.247474), // Cesena: 44.143043, 12.247474);
                DistancePerBatteryPercent: Distance.FromMeters(450),
                UpdateFrequency: UpdateFrequency ?? Duration.FromMinutes(1));
        }
    }
}

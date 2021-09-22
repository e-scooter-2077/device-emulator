using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceEmulator.Model.Values;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using Geolocation;

namespace DeviceEmulator.Model.Entities
{
    public record EScooter
    {
        public Guid Id { get; init; }
        public bool Unsynced { get; init; }
        public bool Locked { get; init; }
        public Speed Speed { get; init; } = 0;
        public Direction Direction { get; init; } = Direction.East;
        public Duration UpdateFrequency { get; init; }
        public Speed MaxSpeed { get; init; }
        public Fraction BatteryLevel { get; init; } = Fraction.FromPercentage(100);
        public Fraction StandbyThreshold { get; init; }
        public Coordinate Position { get; init; } = new Coordinate(44.143043, 12.247474);  // Cesena: 44.143043, 12.247474
    }
}

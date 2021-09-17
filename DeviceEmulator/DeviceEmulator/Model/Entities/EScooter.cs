using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geolocation;
using DeviceEmulator.Model.Values;

namespace DeviceEmulator.Model.Entities
{
    public record EScooter
    {
        public Guid Id { get; init; }
        public bool Locked { get; init; }
        public bool Enabled { get; init; }
        public Duration UpdateFrequency { get; init; }
        public Speed MaxSpeed { get; init; }
        public Percentage BatteryLevel { get; init; }
        public Percentage PowerSavingThreshold { get; init; }
        public Percentage StandbyThreshold { get; init; }
        public Coordinate Position { get; init; }
    }
}

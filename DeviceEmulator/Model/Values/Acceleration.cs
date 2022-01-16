using EasyDesk.CleanArchitecture.Domain.Metamodel.Values;
using System;

namespace DeviceEmulator.Model.Values
{
    public record Acceleration : QuantityWrapper<double>
    {
        private Acceleration(double metersPerSecondSquared) : base(metersPerSecondSquared)
        {
        }

        protected override void Validate(double value)
        {
        }

        public double MetersPerSecondSquared => Value;

        public double KilometersPerHourSquared => MetersPerSecondSquared * 12960;

        public static Acceleration FromMetersPerSecondSquared(double metersPerSecondSquared) => new(metersPerSecondSquared);

        public static Acceleration FromKilometersPerHourSquared(double kilometersPerHourSquared) => FromMetersPerSecondSquared(kilometersPerHourSquared / 12960);

        public static Acceleration FromKilometersPerHourPerSecond(double kilometersPerHourPerSecond) => FromMetersPerSecondSquared(kilometersPerHourPerSecond / 3.6);

        public static SpeedDelta operator *(Acceleration s, TimeSpan t) => SpeedDelta.FromMetersPerSecond(s.MetersPerSecondSquared * t.TotalSeconds);

        public static Acceleration operator *(Acceleration s, double multiplier) => FromMetersPerSecondSquared(s.MetersPerSecondSquared * multiplier);
    }
}

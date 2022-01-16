using EasyDesk.CleanArchitecture.Domain.Metamodel.Values;
using System;

namespace DeviceEmulator.Model.Values
{
    public record Speed : QuantityWrapper<double>
    {

        private Speed(double metersPerSecond) : base(metersPerSecond)
        {
        }

        protected override void Validate(double value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Speed must be positive.");
            }
        }

        public double MetersPerSecond => Value;

        public double KilometersPerHour => MetersPerSecond * 3.6;

        public static Speed FromMetersPerSecond(double metersPerSecond) => new(metersPerSecond);

        public static Speed FromKilometersPerHour(double kilometersPerHour) => FromMetersPerSecond(kilometersPerHour / 3.6);

        public static Distance operator *(Speed s, TimeSpan t) => Distance.FromMeters(s.MetersPerSecond * t.TotalSeconds);

        public static Speed operator *(Speed s, double multiplier) => FromMetersPerSecond(s.MetersPerSecond * multiplier);

        public static Speed operator +(Speed a, Speed b) => FromMetersPerSecond(a.MetersPerSecond + b.MetersPerSecond);

        public static Acceleration operator /(Speed d, TimeSpan t) => Acceleration.FromMetersPerSecondSquared(d.MetersPerSecond / t.TotalSeconds);
    }

    public record SpeedDelta : QuantityWrapper<double>
    {

        private SpeedDelta(double metersPerSecond) : base(metersPerSecond)
        {
        }

        protected override void Validate(double value)
        {
        }

        public double MetersPerSecond => Value;

        public double KilometersPerHour => MetersPerSecond * 3.6;

        public static SpeedDelta FromMetersPerSecond(double metersPerSecond) => new(metersPerSecond);

        public static SpeedDelta FromKilometersPerHour(double kilometersPerHour) => FromMetersPerSecond(kilometersPerHour / 3.6);

        public static SpeedDelta operator *(SpeedDelta s, double multiplier) => SpeedDelta.FromMetersPerSecond(s.MetersPerSecond * multiplier);

        public static SpeedDelta operator +(SpeedDelta a, SpeedDelta b) => FromMetersPerSecond(a.MetersPerSecond + b.MetersPerSecond);

        public static Speed operator +(Speed a, SpeedDelta b) => Speed.FromMetersPerSecond(Math.Max(0, a.MetersPerSecond + b.MetersPerSecond));

        public static Acceleration operator /(SpeedDelta d, TimeSpan t) => Acceleration.FromMetersPerSecondSquared(d.MetersPerSecond / t.TotalSeconds);
    }
}

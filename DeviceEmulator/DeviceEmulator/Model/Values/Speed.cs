using EasyDesk.CleanArchitecture.Domain.Metamodel.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static Speed operator *(Speed s, double multiplier) => Speed.FromMetersPerSecond(s.MetersPerSecond * multiplier);

        public static Speed operator +(Speed a, Speed b) => Speed.FromMetersPerSecond(a.MetersPerSecond + b.MetersPerSecond);

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

        public static implicit operator SpeedDelta(double v) => FromMetersPerSecond(v);

        public static Speed operator *(SpeedDelta s, double multiplier) => Speed.FromMetersPerSecond(s.MetersPerSecond * multiplier);

        public static SpeedDelta operator +(SpeedDelta a, SpeedDelta b) => SpeedDelta.FromMetersPerSecond(a.MetersPerSecond + b.MetersPerSecond);

        public static Speed operator +(Speed a, SpeedDelta b) => Speed.FromMetersPerSecond(Math.Max(0, a.MetersPerSecond + b.MetersPerSecond));

        public static Acceleration operator /(SpeedDelta d, TimeSpan t) => Acceleration.FromMetersPerSecondSquared(d.MetersPerSecond / t.TotalSeconds);
    }
}

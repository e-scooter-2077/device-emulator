using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceEmulator.Model.Values
{
    public record Speed : IComparable<Speed>
    {
        private Speed(double metersPerSecond)
        {
            if (metersPerSecond < 0)
            {
                throw new ArgumentException("Speed must be positive.");
            }
            MetersPerSecond = metersPerSecond;
        }

        public double MetersPerSecond { get; }

        public double KilometersPerHour => MetersPerSecond * 3.6;

        public static Speed FromMetersPerSecond(double metersPerSecond) => new(metersPerSecond);

        public static Speed FromKilometersPerHour(double kilometersPerHour) => FromMetersPerSecond(kilometersPerHour / 3.6);

        public int CompareTo(Speed other) => MetersPerSecond.CompareTo(other.MetersPerSecond);

        public static implicit operator Speed(double v) => FromMetersPerSecond(v);

        public static Distance operator *(Speed s, TimeSpan t) => Distance.FromMeters(s.MetersPerSecond * t.TotalSeconds);

        public static bool operator >(Speed a, Speed b) => a.CompareTo(b) > 0;

        public static bool operator <(Speed a, Speed b) => a.CompareTo(b) < 0;

        public static bool operator >=(Speed a, Speed b) => a.CompareTo(b) >= 0;

        public static bool operator <=(Speed a, Speed b) => a.CompareTo(b) <= 0;

        public static Speed operator *(Speed s, double multiplier) => Speed.FromMetersPerSecond(s.MetersPerSecond * multiplier);
    }
}

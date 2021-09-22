using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceEmulator.Model.Values
{
    public record Distance : IComparable<Distance>
    {
        private Distance(double meters)
        {
            if (meters < 0)
            {
                throw new ArgumentException("Distance can't be negative");
            }
            Meters = meters;
        }

        public double Meters { get; }

        public double Kilometers => Meters / 1000;

        public static Distance FromMeters(double meters) => new(meters);

        public static Distance FromKilometers(double km) => FromMeters(km * 1000);

        public static Speed operator /(Distance d, TimeSpan t) => Speed.FromMetersPerSecond(d.Meters / t.TotalSeconds);

        public static Distance Min(Distance a, Distance b) => a <= b ? a : b;

        public int CompareTo(Distance other) => Meters.CompareTo(other.Meters);

        public static bool operator >(Distance a, Distance b) => a.CompareTo(b) > 0;

        public static bool operator <(Distance a, Distance b) => a.CompareTo(b) < 0;

        public static bool operator >=(Distance a, Distance b) => a.CompareTo(b) >= 0;

        public static bool operator <=(Distance a, Distance b) => a.CompareTo(b) <= 0;
    }
}

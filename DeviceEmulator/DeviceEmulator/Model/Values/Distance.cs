using EasyDesk.CleanArchitecture.Domain.Metamodel.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceEmulator.Model.Values
{
    public record Distance : QuantityWrapper<double>
    {
        private Distance(double meters) : base(meters)
        {
        }

        public double Meters => Value;

        public double Kilometers => Meters / 1000;

        protected override void Validate(double value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Distance can't be negative");
            }
        }

        public static Distance FromMeters(double meters) => new(meters);

        public static Distance FromKilometers(double km) => FromMeters(km * 1000);

        public static Speed operator /(Distance d, TimeSpan t) => Speed.FromMetersPerSecond(d.Meters / t.TotalSeconds);

        public static Distance Min(Distance a, Distance b) => a <= b ? a : b;
    }
}

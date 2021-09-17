using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceEmulator.Model.Values
{
    public record Speed
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

        public static Speed FromKilometersPerHour(double kilometersPerHour) => new(kilometersPerHour / 3.6);
    }
}

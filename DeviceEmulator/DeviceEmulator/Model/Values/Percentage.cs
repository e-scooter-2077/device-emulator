using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceEmulator.Model.Values
{
    public record Percentage
    {
        public int Base100ValueRounded => (int)Math.Round(Base100Value);
        public double Base100Value { get; }
        public double Base1Value => Base100Value / 100;
        private Percentage(double percentage)
        {
            if (percentage < 0 || percentage > 100)
            {
                throw new ArgumentException("Percentage must be between 0 and 100, inclusive.");
            }
            Base100Value = percentage;
        }

        public static Percentage FromFraction(double fraction)
        {
            if (fraction < 0 || fraction > 1)
            {
                throw new ArgumentException("Fraction must be between 0 and 1, inclusive.");
            }
            return new(fraction * 100);
        }

        public static Percentage FromPercentage(double percentage) => new(percentage);
    }
}
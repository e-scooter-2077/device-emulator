using System;
using Rand = System.Random;

namespace DeviceEmulator.Model.Values
{
    public record Direction
    {
        private Direction(double rads)
        {
            var t = rads % (2 * Math.PI);
            Radians = t < 0 ? t + 2 * Math.PI : t;
        }

        public double Radians { get; }

        public double Degrees => Radians / Math.PI * 180;

        public double ClockwiseRadiansFromNorth => (-Radians - 3 * Math.PI / 2) % (2 * Math.PI) + 2 * Math.PI;

        public static Direction FromRadians(double rads) => new(rads);

        public static Direction FromDegrees(double degrees) => FromRadians(degrees / 180 * Math.PI);

        public static Direction North => FromRadians(Math.PI / 2);

        public static Direction South => FromRadians(-Math.PI / 2);

        public static Direction East => FromRadians(0);

        public static Direction West => FromRadians(Math.PI);

        public static Direction Random => FromRadians(new Rand().NextDouble() * 2 * Math.PI);

        public static implicit operator Direction(double v) => FromRadians(v);
    }
}

using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceEmulatorUnitTests.Extensions
{
    public static class DoubleShouldExtensions
    {
        public static void ShouldBeZeroRadians(this double sut, double eps) =>
            (Math.Abs(sut) < eps || Math.Abs(sut - (2 * Math.PI)) < eps).ShouldBeTrue();
    }
}

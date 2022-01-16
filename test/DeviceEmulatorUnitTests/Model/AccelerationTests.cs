using DeviceEmulator.Model.Values;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DeviceEmulatorUnitTests.Model
{
    public class AccelerationTests
    {
        private double _epsilon = 0.000001;

        [Fact]
        public void Acceleration_Conversions_ShouldWork()
        {
            Acceleration.FromMetersPerSecondSquared(100).MetersPerSecondSquared.ShouldBe(100, _epsilon);
            Acceleration.FromMetersPerSecondSquared(100).KilometersPerHourSquared.ShouldBe(1296000, _epsilon);
            Acceleration.FromKilometersPerHourSquared(12960000).MetersPerSecondSquared.ShouldBe(1000, _epsilon);
            Acceleration.FromKilometersPerHourPerSecond(3600).MetersPerSecondSquared.ShouldBe(1000, _epsilon);
        }

        [Fact]
        public void Speed_equals_Acceleration_times_TimeSpan()
        {
            (Acceleration.FromMetersPerSecondSquared(10) * TimeSpan.FromSeconds(2)).MetersPerSecond.ShouldBe(20, _epsilon);
            (Acceleration.FromMetersPerSecondSquared(-10) * TimeSpan.FromSeconds(2)).MetersPerSecond.ShouldBe(-20, _epsilon);
        }

        [Fact]
        public void Acceleration_Multiplication_Should_Work()
        {
            (Acceleration.FromMetersPerSecondSquared(11) * 3).MetersPerSecondSquared.ShouldBe(33, _epsilon);
        }
    }
}

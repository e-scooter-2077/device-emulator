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
    public class SpeedTests
    {
        private double _epsilon = 0.0000001;

        [Fact]
        public void Speed_CanOnlyHavePositiveValues()
        {
            Speed.FromMetersPerSecond(12).ShouldNotBeNull();
            Speed.FromMetersPerSecond(0).ShouldNotBeNull();
            Speed.FromKilometersPerHour(133).ShouldNotBeNull();
            Speed.FromKilometersPerHour(0).ShouldNotBeNull();
            Should.Throw<ArgumentException>(() => Speed.FromMetersPerSecond(-11));
            Should.Throw<ArgumentException>(() => Speed.FromKilometersPerHour(-121));
        }

        [Fact]
        public void Kmh_To_Ms_And_Viceversa_Conversions_ShouldBeCorrect()
        {
            var ms = 4.5;
            var kmh = ms * 3.6;
            Speed.FromMetersPerSecond(ms).KilometersPerHour.ShouldBe(kmh, _epsilon);
            Speed.FromKilometersPerHour(kmh).MetersPerSecond.ShouldBe(ms, _epsilon);
        }

        [Fact]
        public void Distance_equals_speed_times_timespan()
        {
            (Speed.FromMetersPerSecond(11) * TimeSpan.FromSeconds(2)).Meters.ShouldBe(22, _epsilon);
        }

        [Fact]
        public void Speed_Multiplication_ShouldBeCorrect()
        {
            (Speed.FromMetersPerSecond(13) * 3).MetersPerSecond.ShouldBe(39, _epsilon);
        }

        [Fact]
        public void Speed_Sum_ShouldBeCorrect()
        {
            (Speed.FromMetersPerSecond(12) + Speed.FromMetersPerSecond(13)).MetersPerSecond.ShouldBe(25, _epsilon);
        }

        [Fact]
        public void Acceleration_equals_speed_divided_by_timespan()
        {
            (Speed.FromMetersPerSecond(35) / TimeSpan.FromSeconds(7)).MetersPerSecondSquared.ShouldBe(5, _epsilon);
        }

        [Fact]
        public void Kmh_To_Ms_And_Viceversa_Conversions_ShouldBeCorrect_ForSpeedDeltas()
        {
            var ms = 4.5;
            var kmh = ms * 3.6;
            SpeedDelta.FromMetersPerSecond(ms).KilometersPerHour.ShouldBe(kmh, _epsilon);
            SpeedDelta.FromKilometersPerHour(kmh).MetersPerSecond.ShouldBe(ms, _epsilon);
        }

        [Fact]
        public void SpeedDeltas_CanHaveNegativeValues()
        {
            SpeedDelta.FromMetersPerSecond(12).ShouldNotBeNull();
            SpeedDelta.FromMetersPerSecond(0).ShouldNotBeNull();
            SpeedDelta.FromKilometersPerHour(133).ShouldNotBeNull();
            SpeedDelta.FromKilometersPerHour(0).ShouldNotBeNull();
            SpeedDelta.FromMetersPerSecond(-11).ShouldNotBeNull();
            SpeedDelta.FromKilometersPerHour(-121).ShouldNotBeNull();
        }

        [Fact]
        public void SpeedDeltas_Multiplication_ShouldWork()
        {
            (SpeedDelta.FromMetersPerSecond(-12) * 3).MetersPerSecond.ShouldBe(-36, _epsilon);
        }

        [Fact]
        public void SpeedDelta_Sum_ShouldWork()
        {
            (SpeedDelta.FromMetersPerSecond(12) + SpeedDelta.FromMetersPerSecond(11)).MetersPerSecond.ShouldBe(23, _epsilon);
            (SpeedDelta.FromMetersPerSecond(-12) + SpeedDelta.FromMetersPerSecond(11)).MetersPerSecond.ShouldBe(-1, _epsilon);
            (SpeedDelta.FromMetersPerSecond(12) + SpeedDelta.FromMetersPerSecond(-11)).MetersPerSecond.ShouldBe(1, _epsilon);
            (SpeedDelta.FromMetersPerSecond(-12) + SpeedDelta.FromMetersPerSecond(-11)).MetersPerSecond.ShouldBe(-23, _epsilon);
        }

        [Fact]
        public void Acceleration_equals_speedDelta_divided_by_timespan()
        {
            (SpeedDelta.FromMetersPerSecond(-110) / TimeSpan.FromSeconds(11)).MetersPerSecondSquared.ShouldBe(-10, _epsilon);
        }
    }
}

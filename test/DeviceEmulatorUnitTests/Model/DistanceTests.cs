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
    public class DistanceTests
    {
        private double _epsilon = 0.000001;

        [Fact]
        public void Distance_MustHave_PositiveValue()
        {
            Distance.FromMeters(2).ShouldNotBeNull();
            Distance.FromMeters(0).ShouldNotBeNull();
            Should.Throw<ArgumentException>(() => Distance.FromMeters(-1));
        }

        [Fact]
        public void Distance_Units_Conversions_ShouldBeCorrect()
        {
            Distance.FromMeters(1000).Kilometers.ShouldBe(1, _epsilon);
            Distance.FromKilometers(2).Meters.ShouldBe(2000, _epsilon);
        }

        [Fact]
        public void Speed_equals_Distance_divided_by_TimeSpan()
        {
            (Distance.FromMeters(10) / TimeSpan.FromSeconds(2)).MetersPerSecond.ShouldBe(5, _epsilon);
        }

        [Fact]
        public void Min_Should_Return_The_Minimum()
        {
            var bigger = Distance.FromMeters(100);
            var middle = Distance.FromMeters(50);
            var smaller = Distance.FromMeters(10);
            Distance.Min(bigger, middle).ShouldBe(middle);
            Distance.Min(bigger, smaller).ShouldBe(smaller);
            Distance.Min(smaller, middle).ShouldBe(smaller);
            new List<Distance>
            {
                bigger,
                smaller,
                middle
            }.Aggregate((d1, d2) => Distance.Min(d1, d2))
            .ShouldBe(smaller);
        }
    }
}

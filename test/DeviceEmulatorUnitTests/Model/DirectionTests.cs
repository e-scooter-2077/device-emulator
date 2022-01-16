using DeviceEmulator.Model.Values;
using DeviceEmulatorUnitTests.Extensions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DeviceEmulatorUnitTests.Model
{
    public class DirectionTests
    {
        private double _epsilon = 0.0000001;

        [Fact]
        public void Radians_Should_Be_Standardized()
        {
            Direction.FromRadians(3 * Math.PI).Radians.ShouldBe(Math.PI, _epsilon);
            Direction.FromRadians(-Math.PI).Radians.ShouldBe(Math.PI, _epsilon);
        }

        [Fact]
        public void UnitConversions_ShouldBeCorrect()
        {
            Direction.FromRadians(Math.PI).Degrees.ShouldBe(180, _epsilon);
            Direction.FromDegrees(90).Radians.ShouldBe(Math.PI / 2, _epsilon);
            Direction.FromRadians(Math.PI / 2).ClockwiseRadiansFromNorth.ShouldBeZeroRadians(_epsilon);
            Direction.FromRadians(-Math.PI / 2).ClockwiseRadiansFromNorth.ShouldBe(Math.PI, _epsilon);
        }

        [Fact]
        public void CardinalDirections_ShouldBeCorrect()
        {
            Direction.North.ClockwiseRadiansFromNorth.ShouldBeZeroRadians(_epsilon);
            Direction.East.ClockwiseRadiansFromNorth.ShouldBe(Math.PI / 2, _epsilon);
            Direction.South.ClockwiseRadiansFromNorth.ShouldBe(Math.PI, _epsilon);
            Direction.West.ClockwiseRadiansFromNorth.ShouldBe(Math.PI * 3 / 2, _epsilon);
        }
    }
}

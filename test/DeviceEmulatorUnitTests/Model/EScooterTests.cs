using DeviceEmulator.Model.Entities;
using DeviceEmulator.Model.Values;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using Geolocation;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DeviceEmulatorUnitTests.Model
{
    public class EScooterTests
    {
        [Fact]
        public void MaxSpeed_ShouldBe_StandbyMaxSpeed_DuringStandby()
        {
            new EScooter(
                Id: Guid.Empty,
                Locked: false,
                BatteryLevel: Fraction.FromPercentage(30),
                StandbyThreshold: Fraction.FromPercentage(40),
                Acceleration: Acceleration.FromKilometersPerHourPerSecond(0),
                Speed: Speed.FromKilometersPerHour(0),
                StandbyMaxSpeed: Speed.FromKilometersPerHour(10),
                DesiredMaxSpeed: Speed.FromKilometersPerHour(20),
                Direction: Direction.North,
                Position: default(Coordinate),
                DistancePerBatteryPercent: Distance.FromMeters(1),
                UpdateFrequency: Duration.FromSeconds(30))
                .ShouldSatisfyAllConditions(
                e => e.MaxSpeed.ShouldBe(e.StandbyMaxSpeed));
        }

        [Fact]
        public void MaxSpeed_ShouldBe_DesiredMaxSpeed_DuringNormalActivity()
        {
            new EScooter(
                Id: Guid.Empty,
                Locked: false,
                BatteryLevel: Fraction.FromPercentage(30),
                StandbyThreshold: Fraction.FromPercentage(20),
                Acceleration: Acceleration.FromKilometersPerHourPerSecond(0),
                Speed: Speed.FromKilometersPerHour(0),
                StandbyMaxSpeed: Speed.FromKilometersPerHour(10),
                DesiredMaxSpeed: Speed.FromKilometersPerHour(20),
                Direction: Direction.North,
                Position: default(Coordinate),
                DistancePerBatteryPercent: Distance.FromMeters(1),
                UpdateFrequency: Duration.FromSeconds(30))
                .ShouldSatisfyAllConditions(
                e => e.MaxSpeed.ShouldBe(e.DesiredMaxSpeed));
        }

        [Fact]
        public void Standby_ShouldBeTrue_IfBatteryIsUnderThreshold()
        {
            new EScooter(
                Id: Guid.Empty,
                Locked: false,
                BatteryLevel: Fraction.FromPercentage(30),
                StandbyThreshold: Fraction.FromPercentage(40),
                Acceleration: Acceleration.FromKilometersPerHourPerSecond(0),
                Speed: Speed.FromKilometersPerHour(0),
                StandbyMaxSpeed: Speed.FromKilometersPerHour(10),
                DesiredMaxSpeed: Speed.FromKilometersPerHour(20),
                Direction: Direction.North,
                Position: default(Coordinate),
                DistancePerBatteryPercent: Distance.FromMeters(1),
                UpdateFrequency: Duration.FromSeconds(30))
                .Standby.ShouldBeTrue();
            new EScooter(
                Id: Guid.Empty,
                Locked: false,
                BatteryLevel: Fraction.FromPercentage(30),
                StandbyThreshold: Fraction.FromPercentage(20),
                Acceleration: Acceleration.FromKilometersPerHourPerSecond(0),
                Speed: Speed.FromKilometersPerHour(0),
                StandbyMaxSpeed: Speed.FromKilometersPerHour(10),
                DesiredMaxSpeed: Speed.FromKilometersPerHour(20),
                Direction: Direction.North,
                Position: default(Coordinate),
                DistancePerBatteryPercent: Distance.FromMeters(1),
                UpdateFrequency: Duration.FromSeconds(30))
                .Standby.ShouldBeFalse();
        }
    }
}

using DeviceEmulator.Model.Emulation;
using DeviceEmulator.Model.Values;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DeviceEmulatorUnitTests.Model
{
    public class ScooterSettingsTests
    {
        [Fact]
        public void ToEScooterWithDefaults_ShouldFillAllMissingProperties_WithDefaults()
        {
            new ScooterSettings(
                Id: Guid.Empty,
                Unsynced: false,
                Locked: null,
                UpdateFrequency: null,
                MaxSpeed: null)
                .ToEScooterWithDefaults()
                .ShouldSatisfyAllConditions(
                s => s.BatteryLevel.ShouldNotBeNull(),
                s => s.StandbyThreshold.ShouldNotBeNull(),
                s => s.Acceleration.ShouldNotBeNull(),
                s => s.Speed.ShouldNotBeNull(),
                s => s.StandbyMaxSpeed.ShouldNotBeNull(),
                s => s.DesiredMaxSpeed.ShouldNotBeNull(),
                s => s.Direction.ShouldNotBeNull(),
                s => s.DistancePerBatteryPercent.ShouldNotBeNull(),
                s => s.UpdateFrequency.ShouldNotBeNull());
        }

        [Fact]
        public void ToEScooterWithDefaults_ShouldReplaceOnlyMissingProperties_WithDefaults()
        {
            new ScooterSettings(
                Id: Guid.Empty,
                Unsynced: false,
                Locked: null,
                UpdateFrequency: null,
                MaxSpeed: null)
                .ToEScooterWithDefaults()
                .ShouldSatisfyAllConditions(
                s => s.Locked.ShouldBeTrue(),
                s => s.UpdateFrequency.ShouldBe(Duration.FromMinutes(1)),
                s => s.DesiredMaxSpeed.ShouldBe(Speed.FromKilometersPerHour(30)));
        }
    }
}

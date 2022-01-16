using DeviceEmulator.Model.Emulation;
using DeviceEmulator.Model.Entities;
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
    public class EScooterStatusTests
    {
        private EScooter _escooter = new ScooterSettings(
            Id: Guid.Empty,
            Locked: false,
            Unsynced: true,
            UpdateFrequency: Duration.FromSeconds(30),
            MaxSpeed: Speed.FromKilometersPerHour(30)).ToEScooterWithDefaults();

        [Fact]
        public void Constructor_ShouldCheck_NullableArguments()
        {
            Should.Throw<ArgumentNullException>(() => new EScooterStatus(null, Timestamp.Now, Timestamp.Now));
            Should.Throw<ArgumentNullException>(() => new EScooterStatus(_escooter, null, Timestamp.Now));
            Should.Throw<ArgumentNullException>(() => new EScooterStatus(_escooter, Timestamp.Now, null));
        }

        [Fact]
        public void TelemetryCheck_ShouldUpdateLastTelemetry()
        {
            var ts = Timestamp.Now;
            var e = new EScooterStatus(_escooter, ts, ts);
            var after = Timestamp.FromUtcDateTime(ts.AsDateTime.AddSeconds(_escooter.UpdateFrequency.AsTimeSpan.TotalSeconds + 1));
            e.TelemetryCheck(after);
            e.LastTelemetryUpdate.ShouldBe(after);
        }

        [Fact]
        public void TelemetryCheck_ShouldReturnTrue_IfUpdateFrequencyTimeHasPassed()
        {
            var ts = Timestamp.Now;
            var e = new EScooterStatus(_escooter, ts, ts);
            e.TelemetryCheck(ts).ShouldBeFalse();
            var after = Timestamp.FromUtcDateTime(ts.AsDateTime.AddSeconds(_escooter.UpdateFrequency.AsTimeSpan.TotalSeconds + 1));
            e.TelemetryCheck(after).ShouldBeTrue();
        }

        [Fact]
        public void Update_ShouldUpdateLastStatusUpdate()
        {
            var ts = Timestamp.Now;
            var e = new EScooterStatus(_escooter, ts, ts);
            var after = Timestamp.FromUtcDateTime(ts.AsDateTime.AddSeconds(2000));
            e.Update(after);
            e.LastStatusUpdate.ShouldBe(after);
        }

        [Fact]
        public void UpdateFromNewSettings_ShouldUpdateOnlyNonNulls()
        {
            var ts = Timestamp.Now;
            var sut = new EScooterStatus(_escooter, ts, ts);
            var l = sut.Scooter.Locked;
            var d = sut.Scooter.DesiredMaxSpeed;
            var u = sut.Scooter.UpdateFrequency;
            var s = new ScooterSettings(
                Id: Guid.Empty,
                Unsynced: true,
                Locked: null,
                UpdateFrequency: null,
                MaxSpeed: null);
            sut.UpdateFromNewSettings(s);
            sut.Scooter.ShouldBe(_escooter);
            sut = new EScooterStatus(_escooter, ts, ts);

            var s1 = s with { Locked = !l };
            sut.UpdateFromNewSettings(s1);
            sut.ShouldSatisfyAllConditions(
                e => e.Scooter.Locked.ShouldBe(s1.Locked.Value),
                e => e.Scooter.UpdateFrequency.ShouldBe(_escooter.UpdateFrequency),
                e => e.Scooter.DesiredMaxSpeed.ShouldBe(_escooter.MaxSpeed));
            sut = new EScooterStatus(_escooter, ts, ts);

            var s2 = s with { MaxSpeed = d + Speed.FromMetersPerSecond(10) };
            sut.UpdateFromNewSettings(s2);
            sut.ShouldSatisfyAllConditions(
                e => e.Scooter.Locked.ShouldBe(_escooter.Locked),
                e => e.Scooter.UpdateFrequency.ShouldBe(_escooter.UpdateFrequency),
                e => e.Scooter.DesiredMaxSpeed.ShouldBe(s2.MaxSpeed));
            sut = new EScooterStatus(_escooter, ts, ts);

            var s3 = s with { UpdateFrequency = Duration.FromTimeSpan(u.AsTimeSpan.Add(TimeSpan.FromSeconds(10))) };
            sut.UpdateFromNewSettings(s3);
            sut.ShouldSatisfyAllConditions(
                e => e.Scooter.Locked.ShouldBe(_escooter.Locked),
                e => e.Scooter.UpdateFrequency.ShouldBe(s3.UpdateFrequency),
                e => e.Scooter.DesiredMaxSpeed.ShouldBe(_escooter.MaxSpeed));
            sut = new EScooterStatus(_escooter, ts, ts);
        }
    }
}

using DeviceEmulator.Model.Data.Download;
using DeviceEmulator.Model.Emulation;
using DeviceEmulator.Model.Entities;
using DeviceEmulator.Model.Values;
using DeviceEmulatorUnitTests.Mocks;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DeviceEmulatorUnitTests.Web
{
    public class EScooterApiManagerTests
    {
        private CancellationToken _cancellationToken = default;

        [Fact]
        public async Task EScooterApiManager_Should_FetchSettingsFromEScooters()
        {
            var t = new EScooterTwin(
                            Id: Guid.Empty,
                            DesiredDto: new EScooterDesiredDto(
                                Locked: false,
                                UpdateFrequency: "00:00:20",
                                MaxSpeed: 8.3),
                            ReportedDto: new EScooterReportedDto(
                                Locked: false,
                                UpdateFrequency: "00:00:20",
                                MaxSpeed: 8.0,
                                Standby: false));
            var sut = new EScooterApiManager(new IotHubRegistryMock()
            {
                TwinsFetched = () => new List<EScooterTwin>()
                {
                    t
                }
            });
            var res = await sut.FetchEScooterList(_cancellationToken);
            res.ShouldHaveSingleItem();
            res.ShouldContain(t.ToEScooterSettings());
        }

        [Fact]
        public async Task EScooterApiManager_Should_FetchEmptyCollection_WithoutEscooters()
        {
            var sut = new EScooterApiManager(new IotHubRegistryMock()
            {
                TwinsFetched = () => Enumerable.Empty<EScooterTwin>()
            });
            var res = await sut.FetchEScooterList(_cancellationToken);
            res.ShouldBeEmpty();
        }

        [Fact]
        public async Task SendTelemetry_ShouldDelegateToIotHubManager()
        {
            var s = Substitute.For<Action<Guid, EScooterTelemetryDto>>();
            var sut = new EScooterApiManager(new IotHubRegistryMock()
            {
                TelemetryAction = s
            });
            await sut.SendTelemetry(
                new ScooterSettings(
                    Id: Guid.Empty,
                    Locked: false,
                    UpdateFrequency: Duration.FromSeconds(10),
                    MaxSpeed: Speed.FromKilometersPerHour(20),
                    Unsynced: false).ToEScooterWithDefaults(),
                _cancellationToken);
            s.ReceivedCalls().ShouldHaveSingleItem();
        }

        [Fact]
        public async Task UpdateDevice_ShouldDelegateToIotHubManager()
        {
            var s = Substitute.For<Action<Guid, EScooterReportedDto>>();
            var sut = new EScooterApiManager(new IotHubRegistryMock()
            {
                UpdateReportedAction = s
            });
            await sut.UpdateEScooter(
                new ScooterSettings(
                    Id: Guid.Empty,
                    Locked: false,
                    UpdateFrequency: Duration.FromSeconds(10),
                    MaxSpeed: Speed.FromKilometersPerHour(20),
                    Unsynced: false).ToEScooterWithDefaults(),
                _cancellationToken);
            s.ReceivedCalls().ShouldHaveSingleItem();
        }

        [Fact]
        public void ShouldUpdateReportedProperties_ShouldLookForReportedPropertyChanges()
        {
            var a = new ScooterSettings(
                Id: Guid.Empty,
                Unsynced: false,
                Locked: true,
                UpdateFrequency: Duration.FromMinutes(1),
                MaxSpeed: Speed.FromKilometersPerHour(10)).ToEScooterWithDefaults();
            var b = a;
            var c = a with
            {
                Locked = false
            };
            var sut = new EScooterApiManager(new IotHubRegistryMock());
            sut.ShouldUpdateReportedProperties(a, a).ShouldBeFalse();
            sut.ShouldUpdateReportedProperties(a, b).ShouldBeFalse();
            sut.ShouldUpdateReportedProperties(a, c).ShouldBeTrue();
            sut.ShouldUpdateReportedProperties(c, c).ShouldBeFalse();
        }

        [Fact]
        public void DTOConversions_ShouldUseStandardMeasurementUnits()
        {
            var scooter = new ScooterSettings(
                Id: Guid.Empty,
                Unsynced: false,
                Locked: true,
                UpdateFrequency: Duration.FromMinutes(1),
                MaxSpeed: Speed.FromKilometersPerHour(10)).ToEScooterWithDefaults();
            var sut = new EScooterApiManager(new IotHubRegistryMock());
            sut.ConvertEScooterToReportedDto(scooter)
                .ShouldSatisfyAllConditions(
                s => s.UpdateFrequency.ShouldBe(scooter.UpdateFrequency.ToString()),
                s => s.MaxSpeed.ShouldBe(scooter.MaxSpeed.MetersPerSecond));
            sut.ConvertEScooterToTelemetryDto(scooter)
                .ShouldSatisfyAllConditions(
                s => s.BatteryLevel.ShouldBe(scooter.BatteryLevel.Base100Value),
                s => s.Speed.ShouldBe(scooter.Speed.MetersPerSecond));
        }
    }
}

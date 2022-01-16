using DeviceEmulator.Model.Emulation;
using DeviceEmulator.Model.Entities;
using DeviceEmulator.Model.Values;
using EasyDesk.CleanArchitecture.Domain.Time;
using EasyDesk.CleanArchitecture.Infrastructure.Time;
using EasyDesk.CleanArchitecture.Testing;
using EasyDesk.Testing;
using EasyDesk.Tools;
using EasyDesk.Tools.Collections;
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
using Xunit.Abstractions;

namespace DeviceEmulatorUnitTests.Model
{
    public class EScooterEmulatorTests
    {
        private SettableTimestampProvider _timeStampProvider;
        private CancellationToken _cancellationToken = default;

        public EScooterEmulatorTests()
        {
            _timeStampProvider = new SettableTimestampProvider(Timestamp.Now);
        }

        [Fact]
        public async Task UpdateIteration_ShouldAddNewScooters_AndCallUpdate()
        {
            var sut = new EScooterEmulator(_timeStampProvider)
            {
                EscooterSettingsLoader = c => Task.FromResult<IEnumerable<ScooterSettings>>(new List<ScooterSettings>()
                {
                    new ScooterSettings(
                        Id: Guid.Empty,
                        Locked: true,
                        UpdateFrequency: Duration.FromSeconds(30),
                        MaxSpeed: Speed.FromKilometersPerHour(20),
                        Unsynced: false)
                }),
                EScooterUpdatedCallback = Substitute.For<AsyncAction<EScooter, EScooter, CancellationToken>>()
            };
            await sut.EmulateIteration(_cancellationToken, skipPolling: false);
            await sut.EScooterUpdatedCallback.Received()(Arg.Is<EScooter>(e => e == null), Arg.Any<EScooter>(), _cancellationToken);
        }

        [Fact]
        public async Task UpdateIteration_ShouldUpdateUnsyncedScooters_WithNewSettings()
        {
            var sut = new EScooterEmulator(_timeStampProvider)
            {
                EscooterSettingsLoader = c => Task.FromResult<IEnumerable<ScooterSettings>>(new List<ScooterSettings>()
                {
                    new ScooterSettings(
                        Id: Guid.Empty,
                        Locked: true,
                        UpdateFrequency: Duration.FromSeconds(30),
                        MaxSpeed: Speed.FromKilometersPerHour(20),
                        Unsynced: true)
                }),
                EScooterUpdatedCallback = Substitute.For<AsyncAction<EScooter, EScooter, CancellationToken>>()
            };
            await sut.EmulateIteration(_cancellationToken, skipPolling: false);
            await sut.EScooterUpdatedCallback
                .Received()(
                Arg.Any<EScooter>(),
                Arg.Is<EScooter>(e => e != null),
                _cancellationToken);
        }

        [Fact]
        public async Task UpdateIteration_ShouldSendTelemetryOnlyAfterUpdateFrequencyTime()
        {
            var ss = new ScooterSettings(
                        Id: Guid.Empty,
                        Locked: true,
                        UpdateFrequency: Duration.FromSeconds(30),
                        MaxSpeed: Speed.FromKilometersPerHour(20),
                        Unsynced: true);
            var sut = new EScooterEmulator(_timeStampProvider)
            {
                EscooterSettingsLoader = c => Task.FromResult<IEnumerable<ScooterSettings>>(new List<ScooterSettings>()
                {
                    ss
                }),
                EScooterTelemetryCallback = Substitute.For<AsyncAction<EScooter, EScooter, CancellationToken>>()
            };
            await sut.EmulateIteration(_cancellationToken, skipPolling: false);
            await sut.EScooterTelemetryCallback
                .DidNotReceiveWithAnyArgs()(default, default, default);
            _timeStampProvider.Set(t => Timestamp.FromUtcDateTime(t.AsDateTime + ss.UpdateFrequency.AsTimeSpan + TimeSpan.FromSeconds(1)));
            await sut.EmulateIteration(_cancellationToken, skipPolling: true);
            await sut.EScooterTelemetryCallback
                .Received(1)(Arg.Is<EScooter>(e => e != null), Arg.Is<EScooter>(e => e != null), _cancellationToken);
        }

        [Fact]
        public async Task UpdateIteration_ShouldSkipPolling_IfFlagIsTrue()
        {
            var sut = new EScooterEmulator(_timeStampProvider)
            {
                EscooterSettingsLoader = Substitute.For<AsyncFunc<CancellationToken, IEnumerable<ScooterSettings>>>()
            };
            sut.EscooterSettingsLoader(default).ReturnsForAnyArgs(Enumerable.Empty<ScooterSettings>());
            await sut.EmulateIteration(_cancellationToken, skipPolling: true);
            await sut.EscooterSettingsLoader.DidNotReceiveWithAnyArgs()(default);
        }

        [Fact]
        public async Task UpdateIteration_ShouldNotSkipPolling_IfFlagIsFalse()
        {
            var sut = new EScooterEmulator(_timeStampProvider)
            {
                EscooterSettingsLoader = Substitute.For<AsyncFunc<CancellationToken, IEnumerable<ScooterSettings>>>()
            };
            sut.EscooterSettingsLoader(default).ReturnsForAnyArgs(Enumerable.Empty<ScooterSettings>());
            await sut.EmulateIteration(_cancellationToken, skipPolling: false);
            await sut.EscooterSettingsLoader.Received()(_cancellationToken);
        }
    }
}

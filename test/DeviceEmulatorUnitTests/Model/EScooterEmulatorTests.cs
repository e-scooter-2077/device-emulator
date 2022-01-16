using DeviceEmulator.Model.Emulation;
using DeviceEmulator.Model.Entities;
using DeviceEmulator.Model.Values;
using EasyDesk.CleanArchitecture.Domain.Time;
using EasyDesk.CleanArchitecture.Infrastructure.Time;
using EasyDesk.CleanArchitecture.Testing;
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
        private ITimestampProvider _timeStampProvider;
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
            sut.EScooterUpdatedCallback
                .ReceivedCalls()
                .ShouldSatisfyAllConditions(
                    calls => calls.Count().ShouldBeInRange(1, 2),
                    calls => calls.ForEach(call =>
                                call.GetArguments()
                                .ShouldSatisfyAllConditions(
                                    args => args[0].ShouldBeNull(),
                                    args => args[1].ShouldBeOfType<EScooter>())));
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
            sut.EScooterUpdatedCallback
                .ReceivedCalls()
                .ShouldSatisfyAllConditions(
                    calls => calls.Count().ShouldBeInRange(2, 3),
                    calls => calls.ForEach(call =>
                                call.GetArguments()
                                .ShouldSatisfyAllConditions(
                                    args => args[0].ShouldBeNull(),
                                    args => args[1].ShouldBeOfType<EScooter>())));
        }

        [Fact]
        public void UpdateIteration_ShouldSendUpdatesOnlyAfterChanges()
        {

        }

        [Fact]
        public void UpdateIteration_ShouldSendTelemetryOnlyAfterUpdateFrequencyTime()
        {

        }

        [Fact]
        public void UpdateIteration_ShouldSkipPolling_IfFlagIsTrue()
        {

        }

        [Fact]
        public void UpdateIteration_ShouldNotSkipPolling_IfFlagIsFalse()
        {

        }
    }
}

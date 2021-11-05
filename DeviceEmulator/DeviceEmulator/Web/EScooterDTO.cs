using DeviceEmulator.Model.Emulation;
using DeviceEmulator.Model.Entities;
using DeviceEmulator.Model.Values;
using EasyDesk.Tools.Options;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using Geolocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceEmulator.Model.Data.Download
{
    public interface IEScooterProperties
    {
        public bool? Locked { get; }

        public string UpdateFrequency { get; }

        public double? MaxSpeed { get; }
    }

    public record EScooterDesiredDto(
        bool? Locked,
        string UpdateFrequency,
        double? MaxSpeed)
        : IEScooterProperties;

    public record EScooterReportedDto(
        bool? Locked,
        string UpdateFrequency,
        double? MaxSpeed,
        bool? Standby)
        : IEScooterProperties;

    public record EScooterTelemetryDto(
        double? BatteryLevel,
        double? Speed,
        double? Latitude,
        double? Longitude);

    public record EScooterTwin(Guid Id, EScooterDesiredDto DesiredDto, EScooterReportedDto ReportedDto)
    {
        public bool IsUnsynced()
        {
            return !ReportedDto.Locked.HasValue ||
                   ReportedDto.UpdateFrequency is null ||
                   !ReportedDto.MaxSpeed.HasValue ||
                   (DesiredDto.Locked.HasValue && DesiredDto.Locked != ReportedDto.Locked) ||
                   (ReportedDto.UpdateFrequency is not null && DesiredDto.UpdateFrequency != ReportedDto.UpdateFrequency) ||
                   (DesiredDto.MaxSpeed.HasValue && DesiredDto.MaxSpeed != ReportedDto.MaxSpeed);
        }

        public ScooterSettings ToEScooterSettings()
        {
            return new ScooterSettings(
                Id: Id,
                Unsynced: IsUnsynced(),
                Locked: DesiredDto.Locked,
                UpdateFrequency: DesiredDto.UpdateFrequency.AsOption().Map(s => Duration.Parse(s)).OrElseNull(),
                MaxSpeed: DesiredDto.MaxSpeed.AsOption().Map(s => Speed.FromMetersPerSecond(s)).OrElseNull());
        }
    }
}

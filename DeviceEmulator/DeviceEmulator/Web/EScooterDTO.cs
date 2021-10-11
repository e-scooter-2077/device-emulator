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
        int? BatteryLevel,
        double? Speed,
        double? Latitude,
        double? Longitude);

    public record EScooterTwin(Guid Id, EScooterDesiredDto DesiredDto, EScooterReportedDto ReportedDto)
    {
        public bool ShouldUpdate()
        {
            return !((!DesiredDto.Locked.HasValue || DesiredDto.Locked == ReportedDto.Locked) &&
                (DesiredDto.UpdateFrequency == null || DesiredDto.UpdateFrequency == ReportedDto.UpdateFrequency) &&
                (!DesiredDto.MaxSpeed.HasValue || DesiredDto.MaxSpeed == ReportedDto.MaxSpeed));
        }
    }
}

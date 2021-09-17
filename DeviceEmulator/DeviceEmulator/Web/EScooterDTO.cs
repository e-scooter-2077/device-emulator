using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceEmulator.Model.Data.Download
{
    public interface IEScooterDesiredProperties 
    {
        public Guid Id { get; }
        public bool? Locked { get; }
        public bool? Enabled { get; }
        public string UpdateFrequency { get; }
        public double? MaxSpeed { get; }
        public int? PowerSavingThreshold { get; }
        public int? StandbyThreshold { get; }
    }

    public record EScooterDesiredDto(
        Guid Id,
        bool? Locked,
        bool? Enabled,
        string UpdateFrequency,
        double? MaxSpeed,
        int? PowerSavingThreshold,
        int? StandbyThreshold
    ) : IEScooterDesiredProperties;

    public record EScooterReportedDto(
        Guid Id,
        bool? Locked,
        bool? Enabled,
        string UpdateFrequency,
        double? MaxSpeed,
        int? BatteryLevel,
        int? PowerSavingThreshold,
        int? StandbyThreshold,
        double? Latitude,
        double? Longitude
    ) : IEScooterDesiredProperties;

    public record EScooterTwin(EScooterDesiredDto DesiredDto, EScooterReportedDto ReportedDto);
}

using DeviceEmulator.Model.Entities;
using DeviceEmulator.Model.Values;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceEmulator.Model.Emulation
{
    public class EScooterStatus
    {
        public EScooterStatus(
            EScooter scooter,
            Timestamp lastStatusUpdate,
            Timestamp lastTelemetryUpdate)
        {
            Scooter = scooter ?? throw new ArgumentNullException(nameof(scooter));
            LastStatusUpdate = lastStatusUpdate ?? throw new ArgumentNullException(nameof(lastStatusUpdate));
            LastTelemetryUpdate = lastTelemetryUpdate ?? throw new ArgumentNullException(nameof(lastTelemetryUpdate));
        }

        public EScooter Scooter { get; private set; }

        public Timestamp LastStatusUpdate { get; private set; }

        public Timestamp LastTelemetryUpdate { get; private set; }

        public void Update(Timestamp now)
        {
            if (now is null)
            {
                throw new ArgumentNullException(nameof(now));
            }
            Scooter = Scooter.SimulateRandomUsage(Duration.FromTimeOffset(now - LastStatusUpdate));
            LastStatusUpdate = now;
        }

        public bool TelemetryCheck(Timestamp now)
        {
            if (Duration.FromTimeOffset(now - LastTelemetryUpdate) >= Scooter.UpdateFrequency)
            {
                LastTelemetryUpdate = now;
                return true;
            }
            return false;
        }

        public void UpdateFromNewSettings(ScooterSettings settings)
        {
            Scooter = Scooter with
            {
                Locked = settings.Locked ?? Scooter.Locked,
                DesiredMaxSpeed = settings.MaxSpeed ?? Scooter.MaxSpeed,
                UpdateFrequency = settings.UpdateFrequency ?? Scooter.UpdateFrequency
            };
        }
    }
}

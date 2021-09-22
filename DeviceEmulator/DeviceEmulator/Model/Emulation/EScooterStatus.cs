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
    public record EScooterStatus(EScooter Scooter, Timestamp LastStatusUpdate, Timestamp LastTelemetryUpdate);
}

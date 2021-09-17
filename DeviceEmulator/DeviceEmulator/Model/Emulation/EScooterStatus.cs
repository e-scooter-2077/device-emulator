using DeviceEmulator.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceEmulator.Model.Emulation
{
    public record EScooterStatus(
        EScooter Scooter, DateTime LastUpdate);
}

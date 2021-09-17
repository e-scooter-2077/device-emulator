using DeviceEmulator.Model.Entities;
using DeviceEmulator.Web;
using EasyDesk.Tools;
using EasyDesk.Tools.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceEmulator.Model.Emulation
{
    public class EScooterEmulator
    {
        public AsyncFunc<CancellationToken, IEnumerable<EScooter>> EscooterListLoader { get; init; }
        public AsyncFunc<EScooter, CancellationToken, Nothing> EScooterUpdatedCallback { get; init; }

        private IEnumerable<EScooterStatus> _escooterList = new List<EScooterStatus>();

        public async Task EmulateIteration(CancellationToken stoppingToken)
        {
            var resultStatus = new ConcurrentBag<EScooterStatus>();

            Parallel.ForEach(await EscooterListLoader(stoppingToken), async scooter =>
            {
                var lastStatus = _escooterList.Where(status => status.Scooter.Id == scooter.Id).FirstOption();
                if (lastStatus.IsAbsent)
                {
                    resultStatus.Add(new EScooterStatus(scooter, DateTime.Now));
                }
                else
                {
                    var newScooter = ComputeEScooterUpdate(scooter, lastStatus.Value);
                    resultStatus.Add(new EScooterStatus(newScooter, DateTime.Now));
                    await EScooterUpdatedCallback(newScooter, stoppingToken);
                }
            });

            _escooterList = resultStatus;
        }

        private EScooter ComputeEScooterUpdate(EScooter iotHubScooter, EScooterStatus previous)
        {
            return iotHubScooter;
            throw new NotImplementedException();
        }
    }
}

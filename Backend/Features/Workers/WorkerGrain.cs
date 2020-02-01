using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Features.Workers
{
    using Orleans;
    using Backend.Contracts.Features.Workers;

    class WorkerGrain : Grain, IWorkerGrain
    {
        public ValueTask<WorkerStatus> UpdateStatus()
        {
            throw new NotImplementedException();
        }
    }
}

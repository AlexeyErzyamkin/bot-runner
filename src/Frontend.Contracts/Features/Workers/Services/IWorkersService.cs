using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Contracts.Features.Workers.Services
{
    // Each frontend - a grain. It subscribes to master

    public interface IChange
    {
    }

    public class WorkerAddedChange : IChange
    {
    }

    public class WorkerRemovedChange : IChange
    {
    }

    public interface IWorkersService
    {
        void Subscribe(Action<IChange> onChange);
    }
}

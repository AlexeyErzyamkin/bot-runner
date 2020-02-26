using System;
using Backend.Models.Features.Workers;

namespace Backend.Contracts.Features.Workers
{
    public abstract class WorkerUpdate
    {
        public Guid WorkerId { get; }

        public sealed class Update : WorkerUpdate
        {
            public Update(Guid workerId, WorkerModel model) : base(workerId)
            {
                Model = model;
            }

            public WorkerModel Model { get; }
        }

        public sealed class Delete : WorkerUpdate
        {
            public Delete(Guid workerId) : base(workerId)
            {
            }
        }

        private WorkerUpdate(Guid workerId)
        {
            WorkerId = workerId;
        }
    }
}
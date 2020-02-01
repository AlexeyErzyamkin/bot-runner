using Orleans.Streams;

namespace Backend.Features.Workers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Orleans;
    using Orleans.Concurrency;
    using Backend.Contracts.Features.Workers;

    class WorkerIdentity
    {
        public WorkerIdentity(IWorkerGrain worker, DateTime expireTime)
        {
            Worker = worker;
            ExpireTime = expireTime;
        }

        public IWorkerGrain Worker { get; }
        public DateTime ExpireTime { get; set; }
    }

    abstract class WorkerUpdate
    {
        public IWorkerGrain Worker { get; }

        public class Register : WorkerUpdate
        {
            public Register(IWorkerGrain worker)
                : base(worker)
            {
            }
        }

        private WorkerUpdate(IWorkerGrain worker)
        {
            Worker = worker;
        }
    }

    [StatelessWorker]
    class WorkerAcknowledger : Grain, IWorkerAcknowledger
    {
        public const string StreamProvider = "move_me_to_config";
        private static readonly Guid WorkerUpdateStream = Guid.NewGuid();

        private readonly Dictionary<Guid, WorkerIdentity> _workers = new Dictionary<Guid, WorkerIdentity>();

        private IAsyncStream<WorkerUpdate> _workerUpdates;
        private StreamSubscriptionHandle<WorkerUpdate> _workerUpdatesHandle;

        public override async Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider(StreamProvider);
            _workerUpdates = streamProvider.GetStream<WorkerUpdate>(WorkerUpdateStream, null);
            _workerUpdatesHandle = await _workerUpdates.SubscribeAsync();

            return base.OnActivateAsync();
        }

        public ValueTask<Guid> Register()
        {
            var guid = Guid.NewGuid();
            var worker = GrainFactory.GetGrain<IWorkerGrain>(guid);

            _workers.Add(guid, new WorkerIdentity(worker, DateTime.UtcNow.AddMinutes(10)));

            var streamProvider = GetStreamProvider(StreamProvider);
            var stream = streamProvider.GetStream<WorkerUpdate>(WorkerUpdateStream, string.Empty);
            stream.

            return new ValueTask<Guid>(guid);
        }

        public ValueTask<AcknowledgeResult> Acknowledge(Guid instanceId)
        {
            if (_workers.TryGetValue(instanceId, out var identity))
            {
                var now = DateTime.UtcNow;

                if (identity.ExpireTime < now)
                {
                    identity.ExpireTime = now.AddMinutes(10);

                    return new ValueTask<AcknowledgeResult>(new AcknowledgeResult.Ok(identity.Worker));
                }
                else
                {
                    _workers.Remove(instanceId);
                }
            }

            return new ValueTask<AcknowledgeResult>(new AcknowledgeResult.NotRegistered());
        }
    }
}

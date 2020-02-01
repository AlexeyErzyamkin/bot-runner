namespace Backend.Contracts.Features.Workers
{
    using System;
    using System.Threading.Tasks;
    using Orleans;

    public interface IWorkerAcknowledger : IGrainWithIntegerKey
    {
        ValueTask<Guid> Register();
        ValueTask<AcknowledgeResult> Acknowledge(Guid instanceId);
    }

    public abstract class AcknowledgeResult
    {
        public class Ok : AcknowledgeResult
        {
            public Ok(IWorkerGrain worker)
            {
                Worker = worker;
            }

            public IWorkerGrain Worker { get; }
        }

        public class NotRegistered : AcknowledgeResult
        {
        }

        private AcknowledgeResult()
        {
        }
    }
}

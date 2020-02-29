using Orleans.Concurrency;

namespace Backend.Contracts.Features.Workers
{
    [Immutable]
    public class WorkerMuster
    {
        public static WorkerMuster Instance { get; } = new WorkerMuster();

        private WorkerMuster()
        {
        }
    }
}
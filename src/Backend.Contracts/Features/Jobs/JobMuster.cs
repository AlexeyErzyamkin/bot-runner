using Orleans.Concurrency;

namespace Backend.Contracts.Features.Jobs
{
    [Immutable]
    public class JobMuster
    {
        public static JobMuster Instance { get; } = new JobMuster();

        private JobMuster()
        {
        }
    }
}
using System;

namespace Backend.Contracts.Features.Jobs
{
    public class JobUpdate
    {
        public JobUpdate(Guid jobId)
        {
            JobId = jobId;
        }

        public Guid JobId { get; }
    }
}
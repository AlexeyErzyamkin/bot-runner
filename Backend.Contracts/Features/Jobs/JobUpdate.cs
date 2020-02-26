using System;

namespace Backend.Contracts.Features.Jobs
{
    public abstract class JobUpdate
    {
        private JobUpdate(Guid jobId)
        {
            JobId = jobId;
        }

        public Guid JobId { get; }

        public sealed class Muster : JobUpdate
        {
            public Muster(Guid jobId) : base(jobId)
            {
            }
        }

        public sealed class Delete : JobUpdate
        {
            public Delete(Guid jobId) : base(jobId)
            {
            }
        }

        public sealed class Update : JobUpdate
        {
            public Update(Guid jobId) : base(jobId)
            {
            }
        }
    }
}
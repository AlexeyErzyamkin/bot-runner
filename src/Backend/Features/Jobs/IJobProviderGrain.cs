using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;

namespace Backend.Features.Jobs
{
    [Immutable]
    class JobInstanceModel
    {
        public JobInstanceModel(Guid instanceId)
        {
            InstanceId = instanceId;
        }

        public Guid InstanceId { get; }
    }

    interface IJobProviderGrain : IGrainWithGuidKey
    {
        Task<JobInstanceModel> RequestJob();
    }
}
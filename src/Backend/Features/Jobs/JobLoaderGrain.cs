using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Contracts.Features.Jobs;
using Backend.Models.Features.Jobs;
using Orleans;
using Orleans.Concurrency;

namespace Backend.Features.Jobs
{
    [StatelessWorker(2)]
    public class JobLoaderGrain : Grain, IJobLoaderGrain
    {
        private readonly IJobStorage _jobStorage;

        public JobLoaderGrain(IJobStorage jobStorage)
        {
            _jobStorage = jobStorage;
        }

        public Task<List<JobModel>> LoadAll()
        {
            return _jobStorage.LoadAll();
        }

        public Task<JobModel?> Load(Guid id)
        {
            return _jobStorage.Load(id);
        }
    }
}
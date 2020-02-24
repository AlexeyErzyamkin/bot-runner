using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Contracts;
using Backend.Contracts.Features.Jobs;
using Backend.Contracts.Streams;
using Backend.Models.Features.Jobs;
using Orleans;

namespace Frontend.Features.Jobs.Services
{
    public interface IJobService
    {
        Task<List<JobModel>> LoadAllJobs();

        Task<JobModel?> LoadJob(Guid jobId);

        Task UpdateJob(JobModel model);

        // event Action<Guid> Updates;
    }

    public class JobService : IJobService
    {
        private readonly IClusterClient _clusterClient;
        private readonly IJobLoaderGrain _jobLoader;

        // private readonly StreamConsumer<JobUpdate> _streamUpdates;

        public JobService(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
            _jobLoader = clusterClient.GetGrain<IJobLoaderGrain>(0);

            // var sp = clusterClient.GetStreamProvider(Constants.StreamProviderName);
            // _streamUpdates = new StreamConsumer<JobUpdate>(sp, JobConstants.StreamId, JobConstants.UpdatesStreamNs, OnUpdate);
        }

        public Task<List<JobModel>> LoadAllJobs()
        {
            return _jobLoader.LoadAll();
        }

        public Task<JobModel?> LoadJob(Guid jobId)
        {
            return _jobLoader.Load(jobId);
        }

        public async Task UpdateJob(JobModel model)
        {
            var job = _clusterClient.GetGrain<IJobGrain>(model.JobId);
            await job.Update(model);
        }

        // public event Action<Guid> Updates;

        // private Task OnUpdate(JobUpdate item)
        // {
        //     Updates?.Invoke(item.JobId);
        //
        //     return Task.CompletedTask;
        // }
    }
}
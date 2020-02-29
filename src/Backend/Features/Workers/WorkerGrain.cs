using System.Threading.Tasks;
using Backend.Contracts;
using Backend.Contracts.Features.Jobs;
using Backend.Contracts.Streams;
using Backend.Features.Jobs;
using Backend.Models.Features.Workers;
using Microsoft.Extensions.Logging;

namespace Backend.Features.Workers
{
    using Orleans;
    using Backend.Contracts.Features.Workers;

    class WorkerGrain : Grain, IWorkerGrain
    {
        private readonly ILogger<WorkerGrain> _logger;

        private StreamConsumer<WorkerMuster>? _streamMuster;
        private StreamProducer<WorkerUpdate>? _streamUpdates;
        private StreamConsumer<JobAvailable>? _streamJobAvailable;

        private WorkerModel? _model;

        public WorkerGrain(ILogger<WorkerGrain> logger)
        {
            _logger = logger;
        }

        public override async Task OnActivateAsync()
        {
            var sp = GetStreamProvider(Constants.StreamProviderName);

            _streamUpdates = new StreamProducer<WorkerUpdate>(sp, WorkerConstants.StreamId, WorkerConstants.StreamNs.Update);

            _streamMuster = new StreamConsumer<WorkerMuster>(sp, WorkerConstants.StreamId, WorkerConstants.StreamNs.Muster, OnMuster);
            await _streamMuster.Resume();

            _streamJobAvailable = new StreamConsumer<JobAvailable>(sp, JobConstants.StreamId, JobConstants.JobAvailableStreamNs, OnJobAvailable);
            await _streamJobAvailable.Resume();
        }

        public async Task Register()
        {
            _model = new WorkerModel(this.GetPrimaryKey(), "");

            if (!_streamMuster!.IsSubscribed)
            {
                await _streamMuster!.Subscribe();
            }

            if (!_streamJobAvailable!.IsSubscribed)
            {
                await _streamJobAvailable!.Subscribe();
            }

            await _streamUpdates!.Next(new WorkerUpdate.Update(this.GetPrimaryKey(), _model));
        }

        public async Task Unregister()
        {
            _model = null;

            await _streamUpdates!.Next(new WorkerUpdate.Delete(this.GetPrimaryKey()));

            await _streamMuster!.Unsubscribe();
            await _streamJobAvailable!.Unsubscribe();

            DeactivateOnIdle();
        }

        private async Task OnMuster(WorkerMuster muster)
        {
            if (_model != null)
            {
                await _streamUpdates!.Next(new WorkerUpdate.Update(this.GetPrimaryKey(), _model));
            }
        }

        private async Task OnJobAvailable(JobAvailable jobAvailable)
        {
            _logger.LogInformation("Job available");

            var result = await jobAvailable.JobProvider.RequestJob();

            _logger.LogInformation($"Job instance received: {result.InstanceId.ToString()}");
        }
    }
}

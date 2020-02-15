using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Backend.Contracts;
using Backend.Contracts.Features.Jobs;
using Backend.Features.Jobs;
using Orleans.Streams;

namespace Backend.Features.Workers
{
    using Orleans;
    using Backend.Contracts.Features.Workers;

    class WorkerGrain : Grain, IWorkerGrain, IAsyncObserver<JobAvailable>
    {
        private IAsyncStream<JobAvailable>? _jobsStream;
        // private StreamSubscriptionHandle<JobAvailable>? _jobStreamHandle;

        public override async Task OnActivateAsync()
        {
            var sp = GetStreamProvider(Constants.StreamProviderName);
            _jobsStream = sp.GetStream<JobAvailable>(JobsConstants.JobStreamId, JobsConstants.JobAvailableStreamNs);

            foreach (var eachHandle in await _jobsStream.GetAllSubscriptionHandles())
            {
                await eachHandle.ResumeAsync(this);
            }
        }

        public async Task Register()
        {
            await _jobsStream!.SubscribeAsync(this);
        }

        public async Task Unregister()
        {
            foreach (var eachHandle in await _jobsStream!.GetAllSubscriptionHandles())
            {
                await eachHandle.UnsubscribeAsync();
            }
        }

        public ValueTask<WorkerStatus> UpdateStatus()
        {
            throw new NotImplementedException();
        }

        public async Task OnNextAsync(JobAvailable item, StreamSequenceToken? token = null)
        {
            await item.JobProvider.RequestJob();
        }

        public Task OnCompletedAsync()
        {
            return Console.Out.WriteLineAsync(nameof(OnCompletedAsync));
        }

        public Task OnErrorAsync(Exception ex)
        {
            return Console.Out.WriteLineAsync(nameof(OnErrorAsync));
        }
    }
}

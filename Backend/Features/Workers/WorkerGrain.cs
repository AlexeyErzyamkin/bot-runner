using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Backend.Contracts;
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
            _jobsStream = sp.GetStream<JobAvailable>(JobAvailable.StreamId, null);

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

        public Task OnNextAsync(JobAvailable item, StreamSequenceToken? token = null)
        {
            return Console.Out.WriteLineAsync(nameof(OnNextAsync));
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

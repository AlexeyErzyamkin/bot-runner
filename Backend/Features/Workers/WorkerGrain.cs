using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Backend.Contracts;
using Backend.Contracts.Features.Jobs;
using Backend.Contracts.Streams;
using Backend.Features.Jobs;

namespace Backend.Features.Workers
{
    using Orleans;
    using Backend.Contracts.Features.Workers;

    class WorkerGrain : Grain, IWorkerGrain //, IAsyncObserver<JobAvailable>
    {
        // private IAsyncStream<JobAvailable>? _jobsStream;
        // private StreamSubscriptionHandle<JobAvailable>? _jobStreamHandle;

        private readonly StreamConsumer<JobAvailable> _streamConsumerJobAvailable;

        public WorkerGrain()
        {
            var sp = GetStreamProvider(Constants.StreamProviderName);
            _streamConsumerJobAvailable = new StreamConsumer<JobAvailable>(sp, JobConstants.StreamId, JobConstants.JobAvailableStreamNs,
                async i => await i.JobProvider.RequestJob(),
                () => Task.CompletedTask,
                ex => Task.CompletedTask);
        }

        public override async Task OnActivateAsync()
        {
            await _streamConsumerJobAvailable.Resume();

            // var sp = GetStreamProvider(Constants.StreamProviderName);
            // _jobsStream = sp.GetStream<JobAvailable>(JobConstants.StreamId, JobConstants.JobAvailableStreamNs);
            //
            // foreach (var eachHandle in await _jobsStream.GetAllSubscriptionHandles())
            // {
            //     await eachHandle.ResumeAsync(this);
            // }
        }

        public async Task Register()
        {
            await _streamConsumerJobAvailable.Subscribe();

            // await _jobsStream!.SubscribeAsync(this);
        }

        public async Task Unregister()
        {
            await _streamConsumerJobAvailable.Unsubscribe();

            // foreach (var eachHandle in await _jobsStream!.GetAllSubscriptionHandles())
            // {
            //     await eachHandle.UnsubscribeAsync();
            // }
        }

        public ValueTask<WorkerStatus> UpdateStatus()
        {
            throw new NotImplementedException();
        }

        // public async Task OnNextAsync(JobAvailable item, StreamSequenceToken? token = null)
        // {
        //     await item.JobProvider.RequestJob();
        // }
        //
        // public Task OnCompletedAsync()
        // {
        //     return Console.Out.WriteLineAsync(nameof(OnCompletedAsync));
        // }
        //
        // public Task OnErrorAsync(Exception ex)
        // {
        //     return Console.Out.WriteLineAsync(nameof(OnErrorAsync));
        // }
    }
}

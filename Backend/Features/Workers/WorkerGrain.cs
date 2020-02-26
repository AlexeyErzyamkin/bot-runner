using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Backend.Contracts;
using Backend.Contracts.Features.Jobs;
using Backend.Contracts.Streams;
using Backend.Features.Jobs;
using Backend.Models.Features.Workers;

namespace Backend.Features.Workers
{
    using Orleans;
    using Backend.Contracts.Features.Workers;

    class WorkerGrain : Grain, IWorkerGrain //, IAsyncObserver<JobAvailable>
    {
        // private IAsyncStream<JobAvailable>? _jobsStream;
        // private StreamSubscriptionHandle<JobAvailable>? _jobStreamHandle;

        private StreamConsumer<WorkerMuster>? _streamMuster;
        private StreamProducer<WorkerUpdate>? _streamUpdates;

        private WorkerModel? _model;

        // public WorkerGrain()
        // {
        //     var sp = GetStreamProvider(Constants.StreamProviderName);
        //     _streamConsumerJobAvailable = new StreamConsumer<JobAvailable>(sp, JobConstants.StreamId, JobConstants.JobAvailableStreamNs,
        //         async i => await i.JobProvider.RequestJob(),
        //         () => Task.CompletedTask,
        //         ex => Task.CompletedTask);
        // }

        public override async Task OnActivateAsync()
        {
            var sp = GetStreamProvider(Constants.StreamProviderName);

            _streamMuster = new StreamConsumer<WorkerMuster>(sp, WorkerConstants.StreamId, WorkerConstants.StreamNs.Muster, OnMuster);
            await _streamMuster.ResumeOrSubscribe();

            _streamUpdates = new StreamProducer<WorkerUpdate>(sp, WorkerConstants.StreamId, WorkerConstants.StreamNs.Update);

            // _streamConsumerJobAvailable = new StreamConsumer<JobAvailable>(sp, JobConstants.StreamId, JobConstants.JobAvailableStreamNs,);

            // _jobsStream = sp.GetStream<JobAvailable>(JobConstants.StreamId, JobConstants.JobAvailableStreamNs);
            //
            // foreach (var eachHandle in await _jobsStream.GetAllSubscriptionHandles())
            // {
            //     await eachHandle.ResumeAsync(this);
            // }
        }

        public async Task Register()
        {
            _model = new WorkerModel(this.GetPrimaryKey(), "");

            await _streamUpdates!.Next(new WorkerUpdate.Update(this.GetPrimaryKey(), _model));
        }

        public async Task Unregister()
        {
            _model = null;

            await _streamUpdates!.Next(new WorkerUpdate.Delete(this.GetPrimaryKey()));
        }

        private async Task OnMuster(WorkerMuster muster)
        {
            if (_model != null)
            {
                await _streamUpdates!.Next(new WorkerUpdate.Update(this.GetPrimaryKey(), _model));
            }
        }

        // public ValueTask<WorkerStatus> UpdateStatus()
        // {
        //     throw new NotImplementedException();
        // }

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

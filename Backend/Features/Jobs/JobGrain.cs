using Backend.Contracts;
using Orleans.Streams;

namespace Backend.Features.Jobs
{
    using System;
    using System.Threading.Tasks;
    using Orleans;
    using Backend.Contracts.Features.Jobs;

    // class JobGrainConfig
    // {
    //
    //
    //     public string StreamProviderName { get; }
    //
    //     public Guid StreamId { get; }
    // }

    class JobAvailable
    {
        public static readonly Guid StreamId = new Guid("68F4A2BA-4CBD-4DE6-8069-70A2A5544F23");

        public JobAvailable(IJobGrain job)
        {
            Job = job;
        }

        public IJobGrain Job { get; }
    }

    class JobGrain : Grain, IJobGrain, IAsyncObserver<JobMuster>
    {
        private bool _deleted;
        private JobDescription? _description;

        // private readonly JobGrainConfig _config;
        private IAsyncStream<JobAvailable>? _streamJobAvailable;
        private IAsyncStream<JobUpdate>? _streamJobUpdates;
        private IAsyncStream<JobMuster>? _streamJobMuster;

        // public JobGrain(JobGrainConfig config)
        // {
        //     _config = config;
        // }

        public override async Task OnActivateAsync()
        {
            var sp = GetStreamProvider(Constants.StreamProviderName);
            _streamJobAvailable = sp.GetStream<JobAvailable>(JobAvailable.StreamId, null);
            _streamJobUpdates = sp.GetStream<JobUpdate>(JobsConstants.JobStreamId, JobsConstants.UpdatesStreamNs);

            _streamJobMuster = sp.GetStream<JobMuster>(JobsConstants.JobStreamId, JobsConstants.MusterStreamNs);

            if (await _streamJobMuster.GetAllSubscriptionHandles() is {} subscriptions)
            {
                foreach (var eachSub in subscriptions)
                {
                    await eachSub.ResumeAsync(this);
                }
            }

            // if (await _streamJobAvailable.GetAllSubscriptionHandles() is var subscriptions)
            // {
            //     foreach (var eachSub in subscriptions)
            //     {
            //         await eachSub.ResumeAsync();
            //     }
            // }
        }

        public async Task Update(JobDescription description)
        {
            ThrowIfDeleted();

            // switch (description.Start)
            // {
            //     case Start.After after:
            //         break;
            //     case Start.AtTime atTime:
            //         break;
            //     case Start.Now now:
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }

            _description = description;

            await _streamJobUpdates!.OnNextAsync(new JobUpdate.Update(this.GetPrimaryKey()));

            await _streamJobMuster!.SubscribeAsync(this);
        }

        public async Task Delete()
        {
            ThrowIfDeleted();

            if (await _streamJobMuster!.GetAllSubscriptionHandles() is {} subscriptions)
            {
                foreach (var eachSub in subscriptions)
                {
                    await eachSub.UnsubscribeAsync();
                }
            }

            // DELETE UPDATE!
            await _streamJobUpdates!.OnNextAsync(new JobUpdate.Delete(this.GetPrimaryKey()));

            _description = null;
            _deleted = true;

            DeactivateOnIdle();
        }

        // public async Task Start()
        // {
        //     var self = GrainReference.AsReference<IJobGrain>();
        //     var model = new JobAvailable(self);
        //     await _streamJobAvailable!.OnNextAsync(model);
        // }

        async Task IAsyncObserver<JobMuster>.OnNextAsync(JobMuster item, StreamSequenceToken token)
        {
            if (!_deleted)
            {
                await _streamJobUpdates!.OnNextAsync(new JobUpdate.Muster(this.GetPrimaryKey()));
            }
        }

        Task IAsyncObserver<JobMuster>.OnCompletedAsync()
        {
            // throw new NotImplementedException();
            return Task.CompletedTask;
        }

        Task IAsyncObserver<JobMuster>.OnErrorAsync(Exception ex)
        {
            throw new NotImplementedException();
        }

        private void ThrowIfDeleted()
        {
            if (_deleted)
            {
                throw new InvalidOperationException("Call JobActor when it already deleted");
            }
        }
    }
}
using Backend.Contracts;
using Backend.Models.Features.Jobs;
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
        private readonly IJobStorage _storage;

        private bool _deleted;
        // private JobDescription? _description;
        private JobModel? _model;

        // private readonly JobGrainConfig _config;
        private IAsyncStream<JobAvailable>? _streamJobAvailable;
        private IAsyncStream<JobUpdate>? _streamJobUpdates;
        private IAsyncStream<JobMuster>? _streamJobMuster;

        public JobGrain(IJobStorage storage)
        {
            _storage = storage;
        }

        // public JobGrain(JobGrainConfig config)
        // {
        //     _config = config;
        // }

        public override async Task OnActivateAsync()
        {
            var id = this.GetPrimaryKey();
            _model = await _storage.Load(id);

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

        public async Task Update(JobModel model)
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

            if (_model is null)
            {
                await _storage.Insert(model);
                _model = model;

                await _streamJobMuster!.SubscribeAsync(this);
            }
            else
            {
                if (await _storage.Update(model))
                {
                    _model = model;

                    await _streamJobUpdates!.OnNextAsync(new JobUpdate.Update(this.GetPrimaryKey()));
                }
                else
                {
                    throw new Exception($"Error updating job '{this.GetPrimaryKey().ToString()}'");
                }
            }

            // _description = description;
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

            await _streamJobUpdates!.OnNextAsync(new JobUpdate.Delete(this.GetPrimaryKey()));

            // _description = null;
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
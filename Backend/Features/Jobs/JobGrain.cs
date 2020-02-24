using Backend.Contracts;
using Backend.Contracts.Streams;
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

        public JobAvailable(IJobProviderGrain jobProvider)
        {
            JobProvider = jobProvider;
        }

        public IJobProviderGrain JobProvider { get; }
    }

    class JobGrain : Grain, IJobGrain, IJobProviderGrain
    {
        private readonly IJobStorage _storage;

        private bool _deleted;
        // private JobDescription? _description;
        private JobModel? _model;

        // private readonly JobGrainConfig _config;
        // private IAsyncStream<JobAvailable>? _streamJobAvailable;
        private StreamProducer<JobUpdate>? _streamJobUpdates;
        private StreamConsumer<JobMuster>? _streamJobMuster;

        public JobGrain(IJobStorage storage)
        {
            _storage = storage;

            // var sp = GetStreamProvider(Constants.StreamProviderName);
            //
            // _streamJobUpdates = new StreamProducer<JobUpdate>(sp, JobConstants.StreamId, JobConstants.UpdatesStreamNs);
            // _streamJobMuster = new StreamConsumer<JobMuster>(sp, JobConstants.StreamId, JobConstants.MusterStreamNs, OnJobMuster);
        }

        // public JobGrain(JobGrainConfig config)
        // {
        //     _config = config;
        // }

        public override async Task OnActivateAsync()
        {
            var id = this.GetPrimaryKey();
            _model = await _storage.Load(id);

            // var sp = GetStreamProvider(Constants.StreamProviderName);
            // _streamJobAvailable = sp.GetStream<JobAvailable>(JobAvailable.StreamId, null);

            var sp = GetStreamProvider(Constants.StreamProviderName);

            _streamJobUpdates = new StreamProducer<JobUpdate>(sp, JobConstants.StreamId, JobConstants.UpdatesStreamNs);
            _streamJobMuster = new StreamConsumer<JobMuster>(sp, JobConstants.StreamId, JobConstants.MusterStreamNs, OnJobMuster);

            await _streamJobMuster.ResumeOrSubscribe();

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

                // await _streamJobMuster!.SubscribeAsync(this);
            }
            else
            {
                if (await _storage.Update(model))
                {
                    _model = model;

                    await _streamJobUpdates!.Next(new JobUpdate.Update(this.GetPrimaryKey()));
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

            await _streamJobMuster!.Unsubscribe();
            await _streamJobUpdates!.Next(new JobUpdate.Delete(this.GetPrimaryKey()));

            // _description = null;
            _deleted = true;

            DeactivateOnIdle();
        }

        public Task RequestJob()
        {
            return Task.CompletedTask;
        }

        // public async Task Start()
        // {
        //     var self = GrainReference.AsReference<IJobGrain>();
        //     var model = new JobAvailable(self);
        //     await _streamJobAvailable!.OnNextAsync(model);
        // }

        private Task OnJobMuster(JobMuster _) => _streamJobUpdates!.Next(new JobUpdate.Muster(this.GetPrimaryKey()));

        private void ThrowIfDeleted()
        {
            if (_deleted)
            {
                throw new InvalidOperationException("Call JobActor when it already deleted");
            }
        }
    }
}
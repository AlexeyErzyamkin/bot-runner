using Backend.Contracts;
using Backend.Contracts.Streams;
using Backend.Models.Features.Jobs;

namespace Backend.Features.Jobs
{
    using System;
    using System.Threading.Tasks;
    using Orleans;
    using Backend.Contracts.Features.Jobs;

    class JobAvailable
    {
        // public static readonly Guid StreamId = new Guid("68F4A2BA-4CBD-4DE6-8069-70A2A5544F23");

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
        private JobModel? _model;

        private StreamProducer<JobUpdate>? _streamJobUpdates;
        private StreamConsumer<JobMuster>? _streamJobMuster;
        private StreamProducer<JobAvailable>? _streamJobAvailable;

        // TODO Need to be persisted
        private Guid? _instanceId;

        public JobGrain(IJobStorage storage)
        {
            _storage = storage;
        }

        public override async Task OnActivateAsync()
        {
            var id = this.GetPrimaryKey();
            _model = await _storage.Load(id);

            var sp = GetStreamProvider(Constants.StreamProviderName);

            _streamJobUpdates = new StreamProducer<JobUpdate>(sp, JobConstants.StreamId, JobConstants.UpdatesStreamNs);
            _streamJobMuster = new StreamConsumer<JobMuster>(sp, JobConstants.StreamId, JobConstants.MusterStreamNs, OnJobMuster);
            _streamJobAvailable = new StreamProducer<JobAvailable>(sp, JobConstants.StreamId, JobConstants.JobAvailableStreamNs);

            await _streamJobMuster.ResumeOrSubscribe();
        }

        public async Task Update(JobModel model)
        {
            ThrowIfDeleted();

            if (_model is null)
            {
                await _storage.Insert(model);
                _model = model;
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
        }

        public async Task Delete()
        {
            ThrowIfDeleted();

            await _streamJobMuster!.Unsubscribe();
            await _streamJobUpdates!.Next(new JobUpdate.Delete(this.GetPrimaryKey()));

            _deleted = true;

            DeactivateOnIdle();
        }

        public async Task Start()
        {
            _instanceId = Guid.NewGuid();

            await _streamJobAvailable!.Next(new JobAvailable(this));
        }

        public Task Stop()
        {
            _instanceId = default;

            throw new NotImplementedException();
        }

        // TODO Replace response with discriminated union
        public Task<JobInstanceModel> RequestJob()
        {
            if (_instanceId is {} instanceId)
            {
                return Task.FromResult(new JobInstanceModel(instanceId));
            }

            throw new InvalidOperationException("Job not started");
        }

        private Task OnJobMuster(JobMuster _)
            => _streamJobUpdates!.Next(new JobUpdate.Muster(this.GetPrimaryKey()));

        private void ThrowIfDeleted()
        {
            if (_deleted)
            {
                throw new InvalidOperationException("Call JobActor when it already deleted");
            }
        }
    }
}
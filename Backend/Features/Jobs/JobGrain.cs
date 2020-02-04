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

    class JobGrain : Grain, IJobGrain
    {
        // private readonly JobGrainConfig _config;
        private IAsyncStream<JobAvailable>? _streamJobAvailable;

        // public JobGrain(JobGrainConfig config)
        // {
        //     _config = config;
        // }

        public override Task OnActivateAsync()
        {
            var sp = GetStreamProvider(Constants.StreamProviderName);
            _streamJobAvailable = sp.GetStream<JobAvailable>(JobAvailable.StreamId, null);

            // if (await _streamJobAvailable.GetAllSubscriptionHandles() is var subscriptions)
            // {
            //     foreach (var eachSub in subscriptions)
            //     {
            //         await eachSub.ResumeAsync();
            //     }
            // }

            return base.OnActivateAsync();
        }

        public Task Init(JobDescription description)
        {
            switch (description.Start)
            {
                case Start.After after:
                    break;
                case Start.AtTime atTime:
                    break;
                case Start.Now now:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            throw new NotImplementedException();
        }

        public async Task Start()
        {
            var self = GrainReference.AsReference<IJobGrain>();
            var model = new JobAvailable(self);
            await _streamJobAvailable!.OnNextAsync(model);
        }
    }
}
using System;
using System.Threading.Tasks;
using Backend.Contracts.Features.Jobs;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Orleans;
using Protocols.Job;

namespace Backend.GrpcHost.Services
{
    public class JobService : JobProtocol.JobProtocolBase
    {
        private readonly ILogger<JobService> _logger;
        private readonly IGrainFactory _grainFactory;

        public JobService(ILogger<JobService> logger, IGrainFactory grainFactory)
        {
            _logger = logger;
            _grainFactory = grainFactory;
        }

        public override Task<InitResponse> Init(InitRequest request, ServerCallContext context)
        {
            //var job = _grainFactory.GetGrain<IJobGrain>();

            throw new NotImplementedException();
        }
    }
}
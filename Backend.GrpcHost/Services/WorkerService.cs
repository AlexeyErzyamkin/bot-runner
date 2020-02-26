using Grpc.Core;
using System;
    using System.Threading.Tasks;
    using Protocols.Worker;
    using Microsoft.Extensions.Logging;
    using Orleans;
    using Backend.Contracts.Features.Workers;

namespace Backend.GrpcHost.Services
{
    public class WorkerService : WorkerProtocol.WorkerProtocolBase
    {
        private readonly ILogger<WorkerService> _logger;
        private readonly IGrainFactory _grainFactory;

        public WorkerService(ILogger<WorkerService> logger, IGrainFactory grainFactory)
        {
            _logger = logger;
            _grainFactory = grainFactory;
        }

        public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            _logger.LogDebug(nameof(Register));

            var workerId = Guid.NewGuid();
            var worker = _grainFactory.GetGrain<IWorkerGrain>(workerId);
            await worker.Register();

            return new RegisterResponse
            {
                Id = workerId.ToString()
            };
        }

        public override async Task<StatusResponse> UpdateStatus(StatusRequest request, ServerCallContext context)
        {
            _logger.LogDebug("Status");

            var acknowledger = _grainFactory.GetGrain<IWorkerAcknowledger>(0);
            var result = await acknowledger.Acknowledge(Guid.Parse(request.Id));
            switch (result)
            {
                case AcknowledgeResult.Ok ok:
                {
                    // await ok.Worker.UpdateStatus();

                    return new StatusResponse
                    {
                        Result = StatusResponse.Types.ResultCode.Ok,
                        Version = 0,
                        UpdateVersion = 0
                    };
                }

                case AcknowledgeResult.NotRegistered _:
                {
                    return new StatusResponse
                    {
                        Result = StatusResponse.Types.ResultCode.NotRegistered,
                        Version = 0,
                        UpdateVersion = 0
                    };
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

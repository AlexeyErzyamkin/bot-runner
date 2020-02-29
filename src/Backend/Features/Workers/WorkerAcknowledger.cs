// namespace Backend.Features.Workers
// {
//     using System;
//     using System.Collections.Generic;
//     using System.Threading.Tasks;
//     using Orleans;
//     using Orleans.Concurrency;
//     using Orleans.Streams;
//     using Backend.Contracts.Features.Workers;
//
//     class WorkerIdentity
//     {
//         public WorkerIdentity(IWorkerGrain worker, DateTime expireTime)
//         {
//             Worker = worker;
//             ExpireTime = expireTime;
//         }
//
//         public IWorkerGrain Worker { get; }
//         public DateTime ExpireTime { get; set; }
//     }
//
//     abstract class WorkerUpdate
//     {
//         public class Register : WorkerUpdate
//         {
//             public Register(Guid workerId, IWorkerGrain worker, DateTime expireTime)
//             {
//                 WorkerId = workerId;
//                 Worker = worker;
//                 ExpireTime = expireTime;
//             }
//
//             public Guid WorkerId { get; }
//             public IWorkerGrain Worker { get; }
//             public DateTime ExpireTime { get; }
//         }
//
//         public class Promoted : WorkerUpdate
//         {
//             public Promoted(Guid workerId, DateTime expireTime)
//             {
//                 WorkerId = workerId;
//                 ExpireTime = expireTime;
//             }
//
//             public Guid WorkerId { get; }
//             public DateTime ExpireTime { get; }
//         }
//
//         private WorkerUpdate()
//         {
//         }
//     }
//
//     class WorkerIdentities : IAsyncObserver<WorkerUpdate>
//     {
//         private readonly Dictionary<Guid, WorkerIdentity> _workers = new Dictionary<Guid, WorkerIdentity>();
//         private readonly IAsyncStream<WorkerUpdate> _workerUpdates;
//
//         public WorkerIdentities(IAsyncStream<WorkerUpdate> workerUpdates)
//         {
//             _workerUpdates = workerUpdates;
//         }
//
//         public async Task AddIdentity(Guid workerId, IWorkerGrain worker)
//         {
//             var expireTime = DateTime.UtcNow.AddMinutes(10);
//             var identity = new WorkerIdentity(worker, expireTime);
//             _workers.Add(workerId, identity);
//
//             await _workerUpdates.OnNextAsync(new WorkerUpdate.Register(workerId, worker, expireTime));
//         }
//
//         public async Task<IWorkerGrain?> TryGetWorker(Guid workerId, bool promote)
//         {
//             if (_workers.TryGetValue(workerId, out var identity))
//             {
//                 var now = DateTime.UtcNow;
//
//                 if (identity.ExpireTime < now)
//                 {
//                     if (promote)
//                     {
//                         var expireTime = now.AddMinutes(10);
//                         identity.ExpireTime = expireTime;
//
//                         await _workerUpdates.OnNextAsync(new WorkerUpdate.Promoted(workerId, expireTime));
//                     }
//
//                     return identity.Worker;
//                 }
//                 else
//                 {
//                     _workers.Remove(workerId);
//                 }
//             }
//
//             return null;
//         }
//
//         public Task OnNextAsync(WorkerUpdate item, StreamSequenceToken? token = null)
//         {
//             throw new NotImplementedException();
//         }
//
//         public Task OnCompletedAsync()
//         {
//             throw new NotImplementedException();
//         }
//
//         public Task OnErrorAsync(Exception ex)
//         {
//             throw new NotImplementedException();
//         }
//     }
//
//     [StatelessWorker]
//     class WorkerAcknowledger : Grain, IWorkerAcknowledger
//     {
//         public const string StreamProvider = "move_me_to_config";
//         private static readonly Guid WorkerUpdateStream = Guid.NewGuid();
//
//         private IAsyncStream<WorkerUpdate>? _workerUpdates;
//         private StreamSubscriptionHandle<WorkerUpdate>? _workerUpdatesHandle;
//         private WorkerIdentities? _workerIdentities;
//
//         public override async Task OnActivateAsync()
//         {
//             var streamProvider = GetStreamProvider(StreamProvider);
//             _workerUpdates = streamProvider.GetStream<WorkerUpdate>(WorkerUpdateStream, null);
//
//             _workerIdentities = new WorkerIdentities(_workerUpdates);
//
//             _workerUpdatesHandle = await _workerUpdates.SubscribeAsync(_workerIdentities);
//         }
//
//         public override async Task OnDeactivateAsync()
//         {
//             if (_workerUpdatesHandle != null)
//             {
//                 await _workerUpdatesHandle.UnsubscribeAsync();
//
//                 _workerUpdatesHandle = null;
//             }
//
//             _workerIdentities = null;
//             _workerUpdates = null;
//         }
//
//         public async Task<Guid> Register()
//         {
//             var guid = Guid.NewGuid();
//             var worker = GrainFactory.GetGrain<IWorkerGrain>(guid);
//
//             await _workerIdentities!.AddIdentity(guid, worker);
//
//             return guid;
//         }
//
//         public async Task<AcknowledgeResult> Acknowledge(Guid instanceId)
//         {
//             if (await _workerIdentities!.TryGetWorker(instanceId, true) is IWorkerGrain worker)
//             {
//                 return new AcknowledgeResult.Ok(worker);
//             }
//
//             return new AcknowledgeResult.NotRegistered();
//         }
//     }
// }
